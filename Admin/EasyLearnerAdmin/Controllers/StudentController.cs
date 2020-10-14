using EasyLearner.Service.Dto;
using EasyLearner.Service.Enums;
using EasyLearner.Service.Exception;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Models;
using EasyLearnerAdmin.Utility.Common;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class StudentController : BaseController<StudentController>
    {
        #region Fields
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;
        private readonly RoleManager<Role> _roleManager;
        private readonly IGradeService _gradeService;
        private readonly IMembershipService _membershipService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly ILogService _staffLog;
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Ctor
        public StudentController(UserManager<ApplicationUser> userManager, ILogService staffLog, ISubscriptionTypeService subscriptionTypeService, IMembershipService membershipService, IStudentService studentService, IUserService userService, RoleManager<Role> roleManager, IGradeService gradeService)
        {
            _studentService = studentService;
            _userService = userService;
            _roleManager = roleManager;
            _userManager = userManager;
            _gradeService = gradeService;
            _membershipService = membershipService;
            _subscriptionTypeService = subscriptionTypeService;
            _staffLog = staffLog;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FilterStudent()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetFilterStudentList(JQueryDataTableParamModel param, StudentFilterDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });

                    if (model.IsMember == true && model.IsNotMember == false)
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@MemberShipFilter", SqlDbType.Bit) { Value = true });
                    }
                    //if (model.IsNotMember == true && model.IsMember == false)
                    //{
                    //    parameters.Parameters.Insert(2, new SqlParameter("@MemberShipFilter", SqlDbType.VarChar) { Value = "" });
                    //}
                    if (model.IsNotMember == true && model.IsMember == false)
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@MemberShipFilter", SqlDbType.Bit) { Value =false });
                    }
                    if (model.IsNotMember == true && model.IsMember == true)
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@MemberShipFilter", SqlDbType.Bit) { Value = DBNull.Value });
                    }

                    var allList = await _studentService.GetFilterStudentList(parameters.Parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    ErrorLog.AddErrorLog(ex, "GetFilterStudentList");
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentList(JQueryDataTableParamModel param)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    var allList = await _studentService.GetStudentList(parameters.Parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    ErrorLog.AddErrorLog(ex, "GetStudentList");
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }

        [HttpGet]
        public IActionResult ViewStudent(long id)
        {
            var result = _studentService.GetSingle(x => x.Id == id);
            var studentResult = _userService.GetSingle(x => x.Id == result.UserId);
            BindGradeList(result.GradeId);

            var studentDto = new StudentDto()
            {
                StudentName = result.StudentName,
                UserName = studentResult.UserName,
                Id = result.Id

            };
            var tempView = Mapper.Map<StudentDto>(studentDto);
            return View("ViewStudent", tempView);
        }

        public IActionResult FilterStudentGradeWise(StudentFilterDto model)
        {
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetGradeWiseStudentFilter(JQueryDataTableParamModel param, StudentFilterDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });
                    //parameters.Parameters.Insert(2, new SqlParameter("@Ismember", SqlDbType.Bit) { Value = model.IsMember });

                    var allList = await _studentService.GetGradeWiseStudentFilter(parameters.Parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    ErrorLog.AddErrorLog(ex, "GetGradeWiseStudentFilter");
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }

        [HttpGet]
        public IActionResult StudentAccount(long id)
        {
            var result = _studentService.GetSingle(x => x.Id == id);
            var studentResult = _userService.GetSingle(x => x.Id == result.UserId);
            BindGradeList(result.GradeId);

            var studentDto = new StudentDto()
            {
                StudentName = result.StudentName,
                UserName = studentResult.UserName,
                Id = result.Id
            };
            //var tempView = Mapper.Map<StudentDto>(studentDto);
            return View("StudentAccount", studentDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudentName(long id, string studentName)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _studentService.GetSingle(x => x.Id == id);
                    result.StudentName = studentName;
                    var updateResult = await _studentService.UpdateAsync(result, Accessor, User.GetUserId());
                    if (updateResult != null)
                    {
                        //StaffLog  
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateStudentName }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateStudentName);

                    }
                    else
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);

                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "EditStudentName");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> EditStudentGrade(long id, long gradeId)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _studentService.GetSingle(x => x.Id == id && x.IsDelete == false && x.IsActive == true);
                    if (result != null)
                    {
                        result.GradeId = gradeId;
                        var updateResult = await _studentService.UpdateAsync(result, Accessor, User.GetUserId());
                        if (updateResult != null)
                        {
                            //StaffLog  
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateStudentGrade }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateStudentGrade);

                        }
                        else
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                        }
                    }
                    else
                    {
                        return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "EditStudentPassword");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult _EditPassword(long id)
        {
            var studentDto = new StudentDto()
            {
                Id = id
            };
            return View(@"Components/_EditPassword", studentDto);
        }




        [HttpPost]
        public async Task<IActionResult> EditPassword(StudentDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _studentService.GetSingle(x => x.Id == model.Id);
                    var userResult = _userService.GetSingle(x => x.Id == result.UserId);
                    userResult.PasswordHash = model.UserPassword;
                    var updateResult = await _userService.UpdateAsync(userResult, Accessor, User.GetUserId());
                    if (updateResult != null)
                    {
                        //StaffLog  
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateStudentPassword }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateStudentPassword);
                    }
                    else
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "EditStudentPassword");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        public IActionResult GetFriendList(long id)
        {
            var result = _studentService.GetSingle(x => x.Id == id);
            var studentResult = _userService.GetSingle(x => x.Id == result.UserId);
            var studentDto = new StudentDto()
            {
                StudentName = result.StudentName,
                UserName = studentResult.UserName,
                Id = result.Id
            };
            var tempView = Mapper.Map<StudentDto>(studentDto);
            return View("GetFriendList", tempView);
        }

        [HttpGet]
        public IActionResult StudentSubscription(long id)
        {
            var result = _studentService.GetSingle(x => x.Id == id && x.IsActive == true && x.IsDelete == false);
            var studentDto = new StudentDto();
            if (result != null)
            {
                var studentUserResult = _userService.GetSingle(x => x.Id == result.UserId);
                var subscriptionStudent = _membershipService.GetSingle(x => x.StudentId == id);
                if (subscriptionStudent != null)
                {
                    var subscriptionStudentData = _subscriptionTypeService.GetSingle(x => x.Id == subscriptionStudent.MemberShipTypeId);
                    if (subscriptionStudentData != null)
                    {
                        studentDto = new StudentDto()
                        {
                            StudentName = result.StudentName,
                            UserName = studentUserResult.UserName,
                            Id = result.Id,
                            MembershipStatus=subscriptionStudent.MembershipStatus,
                            NoOfDays = subscriptionStudentData.AllowedDays,
                            NoOfQuestions = subscriptionStudentData.AllowedQuestion,
                        };
                    }
                }

            }
            return View(@"StudentSubscription", studentDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudentNoOfQuestions(long id, int NoOfQuestions)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var subscriptionData = _subscriptionTypeService.GetSingle(x => x.TypeId == id);
                    if (subscriptionData != null)
                    {
                        subscriptionData.AllowedQuestion = NoOfQuestions;
                        var updateResult = await _subscriptionTypeService.UpdateAsync(subscriptionData, Accessor, User.GetUserId());
                        if (updateResult != null)
                        {
                            //StaffLog  
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.StudentMembeshipNoOfQuestionUpdated }, Accessor, User.GetUserId());
                            txscope.Complete();

                        }
                        else
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                        }

                    }
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.StudentMembeshipNoOfQuestionUpdated);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "EditStudentNoOfQuestions");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditStudentNoOfDays(long id, int NoOfDays)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var subscriptionData = _subscriptionTypeService.GetSingle(x => x.TypeId == id);
                    if (subscriptionData != null)
                    {
                        subscriptionData.AllowedDays = NoOfDays;
                        var updateResult = await _subscriptionTypeService.UpdateAsync(subscriptionData, Accessor, User.GetUserId());
                        if (updateResult != null)
                        {
                            //Membership new changes 06-10-2020
                            var membershipData = _membershipService.GetSingle(x=>x.StudentId==id);
                            if (membershipData != null)
                            {
                                membershipData.ExprireDate = DateTime.UtcNow.AddDays(NoOfDays);
                            }
                            if (membershipData.ExprireDate > DateTime.Now)
                            {
                                membershipData.MembershipStatus = true;//For Member Student
                            }
                            if (membershipData.ExprireDate <= DateTime.Now)
                            {
                                membershipData.MembershipStatus = false;//For Non-Member Student
                            }
                            //End

                            //StaffLog  
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.StudentMembeshipNoOfDaysUpdated }, Accessor, User.GetUserId());
                            txscope.Complete();
                        }
                        else
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                        }
                    }
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.StudentMembeshipNoOfDaysUpdated);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "EditStudentNoOfDays");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditStudentMemberShipStatus(long id, bool IsMemberStatus)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //var result = _studentService.GetSingle(x => x.Id == id);
                    //var updateResult = await _studentService.UpdateAsync(result, Accessor, User.GetUserId());
                    //if (updateResult != null)
                    //{
                    //    //StaffLog  
                    //    if (User.IsInRole(UserRoles.Staff))
                    //        await _staffLog.InsertAsync(new Log { StaffId = User.GetUserId(), Description = ResponseConstants.StudentMembeshipStatusUpdated }, Accessor, User.GetUserId());
                    //    txscope.Complete();
                    //    return JsonResponse.GenerateJsonResult(1, ResponseConstants.StudentMembeshipStatusUpdated);
                    //}
                    //else
                    //{
                    //    txscope.Dispose();
                    //    return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                    //}

                    //New changes 06-10-2020
                    var result =_membershipService.GetSingle(x => x.StudentId == id);
                    if (result != null)
                    {
                        result.MembershipStatus = IsMemberStatus;
                    }
                    var updateResult = await _membershipService.UpdateAsync(result, Accessor, User.GetUserId());
                    if (updateResult != null)
                    {
                        //StaffLog  
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.StudentMembeshipStatusUpdated }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.StudentMembeshipStatusUpdated);
                    }
                    else
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "EditStudentNoOfDays");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentFriendList(JQueryDataTableParamModel param, StudentDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    parameters.Parameters.Insert(0, new SqlParameter("@StudentId", SqlDbType.Int) { Value = model.Id });

                    var allList = await _studentService.GetStudentInviteFriendList(parameters.Parameters.ToArray());

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = allList
                    });
                }
                catch (Exception ex)
                {
                    ErrorLog.AddErrorLog(ex, "StudentInvitationList");
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetQAByGrade(JQueryDataTableParamModel param, StudentFilterDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });

                    var allList = await _studentService.GetQAByGrade(parameters.Parameters.ToArray());
                    List<QAGradeStudent> qAGradeStudents = new List<QAGradeStudent>();

                    foreach (var item in allList.GroupBy(x => x.GradeName))
                    {
                        QAGradeStudent qAGradeStudent = new QAGradeStudent();
                        qAGradeStudent.GradeName = item.Key;
                        foreach (var item1 in item)
                        {
                            LessonData lessonData = new LessonData();
                            lessonData.LessonName = item1.LessonName;
                            lessonData.QACount = item1.QACount;
                            qAGradeStudent.Lessons.Add(lessonData);
                        }
                        qAGradeStudents.Add(qAGradeStudent);
                    }

                    var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = total,
                        iTotalDisplayRecords = total,
                        aaData = qAGradeStudents
                    });
                }
                catch (Exception ex)
                {
                    ErrorLog.AddErrorLog(ex, "GetQAByGrade");
                    return Json(new
                    {
                        param.sEcho,
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        aaData = ""
                    });
                }
            }
        }

        public IActionResult NoOfQuestionAnswerGradeWise(StudentFilterDto model)
        {
            return View(model);
        }

        [HttpGet]
        public IActionResult _AddEditStudent(long id)
        {
            BindGradeList(0);
            if (id == 0) return base.View(@"Components/_AddEditStudent", new StudentDto { Id = id });
            var result = _studentService.GetSingle(x => x.Id == id);
            var studentResult = _userService.GetSingle(x => x.Id == result.UserId);
            BindGradeList(result.GradeId);

            var studentDto = new StudentDto()
            {
                PaymentAmount = result.PaymentAmount,
                PaymentExplanation = result.PaymentExplanation,
                RegistrationDate = result.CreatedDate,
                StudentName = result.StudentName,
                StudentStatus = result.StudentStatus,
                UserName = studentResult.UserName,
                //Subscription = result.Subscription,
                Id = result.Id
                // UserPassword =studentResult.PasswordHash
            };
            var tempView = Mapper.Map<StudentDto>(studentDto);
            return View(@"Components/_AddEditStudent", tempView);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditStudent(StudentDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        ErrorLog.AddErrorLog(new Exception("ModelStateIsValid"), "CreateTutor");
                        return RedirectToAction(@"Components/_AddEditStudent", model.Id);
                    }

                    if (model.Id == 0)
                    {
                        if (model.UserName != null)
                        {
                            bool isUserExist = UserNameExists(model.UserName);
                            if (isUserExist != true)
                            {
                                //Created by company staff
                                ApplicationUser studentUser = new ApplicationUser();
                                studentUser.UserName = model.UserName;
                                studentUser.MobileNumber = model.UserName;
                                studentUser.IsActive = true;
                                var createStudentUser = await _userManager.CreateAsync(studentUser, model.UserPassword);
                                if (createStudentUser.Succeeded)
                                {
                                    var isRoleAssgined = await _userManager.AddToRoleAsync(studentUser, UserRoles.Student);
                                    var studentObj = Mapper.Map<Students>(model);
                                    studentObj.CreatedDate = model.RegistrationDate;
                                    studentObj.UserId = studentUser.Id;
                                    studentObj.IsActive = true;
                                  
                                    var result = await _studentService.InsertAsync(studentObj, Accessor, User.GetUserId());
                                    if (result != null)
                                    {
                                        //memebership entry
                                        long MemberShipTypeId = 0;
                                        Membership membershipObj = new Membership();
                                        membershipObj.StudentId = result.Id;
                                        if ((int)GlobalEnums.MemberShipTypes.Annual_and_Normal == Convert.ToInt32(model.Subscription))
                                        {
                                            MemberShipTypeId = (long)GlobalEnums.MemberShipTypes.Annual_and_Normal;
                                            //membershipObj.ExprireDate = DateTime.UtcNow.AddDays(365);
                                        }
                                        if ((int)GlobalEnums.MemberShipTypes.Annual_and_Plus == Convert.ToInt32(model.Subscription))
                                        {
                                            MemberShipTypeId = (long)GlobalEnums.MemberShipTypes.Annual_and_Plus;
                                            //membershipObj.ExprireDate = DateTime.UtcNow.AddDays(365);
                                        }
                                        if ((int)GlobalEnums.MemberShipTypes.Monthly_and_Normal == Convert.ToInt32(model.Subscription))
                                        {
                                            MemberShipTypeId = (long)GlobalEnums.MemberShipTypes.Monthly_and_Normal;
                                            //membershipObj.ExprireDate = DateTime.UtcNow.AddDays(30);
                                        }
                                        if ((int)GlobalEnums.MemberShipTypes.Monthly_and_Plus == Convert.ToInt32(model.Subscription))
                                        {
                                            MemberShipTypeId = (long)GlobalEnums.MemberShipTypes.Monthly_and_Plus;
                                            //membershipObj.ExprireDate = DateTime.UtcNow.AddDays(30);
                                        }
                                        var subscriptionResult = _subscriptionTypeService.GetSingle(x => x.TypeId == MemberShipTypeId);
                                        if (subscriptionResult != null)
                                        {
                                            membershipObj.ExprireDate = DateTime.UtcNow.AddDays(subscriptionResult.AllowedDays);
                                            membershipObj.MemberShipTypeId = subscriptionResult.Id;
                                        }
                                        if (membershipObj.ExprireDate > DateTime.Now)
                                        {
                                            membershipObj.MembershipStatus = true;//For Member Student
                                        }
                                        if(membershipObj.ExprireDate <= DateTime.Now)
                                        {
                                            membershipObj.MembershipStatus = false;//For Non-Member Student
                                        }

                                        membershipObj.IsActive = true;
                                        var memberShipResult = await _membershipService.InsertAsync(membershipObj, Accessor, User.GetUserId());

                                        //StaffLog
                                        if (User.IsInRole(UserRoles.Staff))
                                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewStudent }, Accessor, User.GetUserId());
                                        txscope.Complete();
                                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateNewStudent);
                                    }
                                }

                            }
                            else
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(3, ResponseConstants.UserExist);
                            }
                        }
                        else
                        {
                            var studentObj = Mapper.Map<Students>(model);
                            var result = await _studentService.InsertAsync(studentObj, Accessor, User.GetUserId());
                            if (result != null)
                            {
                                //StaffLog
                                if (User.IsInRole(UserRoles.Staff))
                                    await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewStudent }, Accessor, User.GetUserId());
                                txscope.Complete();
                                return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateNewStudent);
                            }
                            else
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                            }


                        }
                    }
                    else
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                    }

                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "CreateStudent");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> ManageStudentIsActive(long id)
        {
            try
            {
                var studentObj = _studentService.GetSingle(x => x.Id == id);
                studentObj.IsActive = !studentObj.IsActive;
                await _studentService.UpdateAsync(studentObj, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"Student {(studentObj.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-/Student/ManageStudentIsActive");
                return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
            }

        }

        [HttpPost]
        public async Task<IActionResult> RemoveStudents(long[] id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var item in id)
                    {
                        var studentObj = _studentService.GetSingle(x => x.Id == item);
                        if (studentObj.Id != 0)
                        {
                            studentObj.IsDelete = true;
                            await _studentService.UpdateAsync(studentObj, Accessor, User.GetUserId());
                            _studentService.Save();
                        }
                    }

                    //StaffLog  
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.DeleteStudent }, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.DeleteStudent);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-Student Delete");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        public bool UserNameExists(string userName)
        {
            //var userData = _userService.GetSingle(x => x.UserName == userName && x.MobileNumber== userName);
            var userData = _userService.GetSingle(x => x.MobileNumber == userName);
            return userData != null ? true : false;
        }

        #region DropDown
        public void BindGradeList(long id = 0)
        {
            if (id == 0)
            {
                ViewBag.GradeList = _gradeService.GetAll().Where(a => a.IsDelete == false).Select(a =>
                                        new SelectListItem()
                                        {
                                            Value = a.Id.ToString(),
                                            Text = a.Name,
                                        }).OrderBy(a => a.Text).ToList();

            }
            else
            {
                List<SelectListItem> lstGradeItems = new List<SelectListItem>();

                var lstSelectedGrade = new List<SelectListItem>();
                lstSelectedGrade = _gradeService.GetAll().Where(a => a.IsDelete == false && a.Id == id).Select(a =>
                                         new SelectListItem()
                                         {
                                             Selected = true,
                                             Value = a.Id.ToString(),
                                             Text = a.Name,
                                         }).OrderBy(a => a.Text).ToList();

                foreach (var i in lstSelectedGrade)
                {
                    var data = lstGradeItems.Where(x => x.Value == i.Value).Count();
                    if (data == 0)
                        lstGradeItems.Add(i);
                }


                var lstUnSelectedGrade = new List<SelectListItem>();
                lstUnSelectedGrade = _gradeService.GetAll().Where(x => x.Id != id && x.IsDelete==false).Select(a =>
                                        new SelectListItem()
                                        {
                                            Value = a.Id.ToString(),
                                            Text = a.Name,
                                            Selected = false
                                        }).OrderBy(a => a.Text).ToList();
                foreach (var i in lstUnSelectedGrade)
                {
                    var data = lstGradeItems.Where(x => x.Value == i.Value).Count();
                    if (data == 0)
                        lstGradeItems.Add(i);
                }
                ViewBag.GradeList = lstGradeItems;
            }

        }
        #endregion

        #endregion
    }
}