using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Exception;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Utility;
using EasyLearnerAdmin.Utility.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EasyLearner.Service;
using Microsoft.AspNetCore.Identity;
using EasyLearnerAdmin.Data.DbContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using System.Globalization;
using EasyLearner.Service.Enums;
using EasyLearnerAdmin.Models;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Data;
using Newtonsoft.Json;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class TutorController : BaseController<TutorController>
    {
        #region Field
        private readonly ITutorService _tutorService;

        private readonly IUserService _userService;
        private readonly RoleManager<Role> _roleManager;
        private readonly IGradeService _gradeService;
        private readonly ILessonService _lessonService;
        private readonly ITutorRelevantLesson _relevantLessonService;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogService _staffLog;
        #endregion

        #region Ctor
        public TutorController(ITutorService tutorService, ILogService staffLog, ITutorRelevantLesson relevantLesson, ILessonService lessonService, UserManager<ApplicationUser> userManager, IUserService userService, RoleManager<Role> roleManager, IGradeService gradeService)
        {
            _tutorService = tutorService;
            _userService = userService;
            _roleManager = roleManager;
            _userManager = userManager;
            _gradeService = gradeService;
            _lessonService = lessonService;
            _relevantLessonService = relevantLesson;
            _staffLog = staffLog;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Tutorfilter()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetFilterTutorList(JQueryDataTableParamModel param, TutorFiltorDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    if (model.TutorisActive == true && model.TutorisInActive == false)
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@IsActive", SqlDbType.Bit) { Value = true });
                    }
                    if (model.TutorisInActive == true && model.TutorisActive == false)
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@IsActive", SqlDbType.Bit) { Value = false });
                    }

                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });

                    var allList = await _tutorService.GetFilterTutorReport(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetFilterTutorReport");
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

        public IActionResult TutorProfile(long id)
        {
            var result = _tutorService.GetSingle(x => x.Id == id);
            var tutorUserResult = _userService.GetSingle(x => x.Id == result.UserId);
            var tutorDto = new TutorDto()
            {
                TutorName = result.TutorName,
                UserName = tutorUserResult.UserName,
                IsActive = result.IsActive,
                Id = result.Id
            };
            var tempView = Mapper.Map<TutorDto>(tutorDto);
            return View("TutorProfile", tempView);
        }

        [HttpPost]
        public async Task<IActionResult> EditTutorStatus(long id, bool IsActive)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _tutorService.GetSingle(x => x.Id == id);
                    result.IsActive = IsActive;
                    var updateResult = await _tutorService.UpdateAsync(result, Accessor, User.GetUserId());
                    if (updateResult != null)
                    {

                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateTutorStatus }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateTutorStatus);
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
                    ErrorLog.AddErrorLog(ex, "EditTutorStatus");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }

            }
        }


        [HttpPost]
        public async Task<IActionResult> EditTutorName(long id, string TutorName)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _tutorService.GetSingle(x => x.Id == id);
                    result.TutorName = TutorName;
                    var updateResult = await _tutorService.UpdateAsync(result, Accessor, User.GetUserId());
                    if (updateResult != null)
                    {
                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateTutorName }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateTutorName);
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
                    ErrorLog.AddErrorLog(ex, "EditTutorName");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }

            }
        }

        [HttpPost]
        public async Task<IActionResult> EditTutorMoNumber(long id, string Monumber)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!string.IsNullOrEmpty(Monumber))
                    {
                        bool mobileNoExist = MobileNoExists(Monumber);
                        if (mobileNoExist == true)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(3, ResponseConstants.MobileNoExist);
                        }
                        else
                        {
                            var result = _tutorService.GetSingle(x => x.Id == id);
                            var tutorUserResult = _userService.GetSingle(x => x.Id == result.UserId);
                            tutorUserResult.MobileNumber = Monumber;
                            var updateResult = await _userService.UpdateAsync(tutorUserResult, Accessor, User.GetUserId());
                            if (updateResult != null)
                            {
                                //StaffLog
                                if (User.IsInRole(UserRoles.Staff))
                                    await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateTutorMobileNumber }, Accessor, User.GetUserId());
                                txscope.Complete();
                                return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateTutorMobileNumber);
                            }
                        }
                    }
                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "EditTutorMoNumber");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }

            }
        }

        public IActionResult ReportOfNumberOfAnswer(long id)
        {
            //var tutorDto = new TutorDto()
            //{
            //    Id = id
            //};
            ViewBag.Id = id;
            return View("ReportOfNumberOfAnswer");
        }

        public IActionResult TutorAccount(long id)
        {
            if (id == 0)
            {
                BindLessonList(null);
                return base.View(@"Components/_AddEditTutor", new TutorDto { Id = id });
            }
            var result = _tutorService.GetSingle(x => x.Id == id);
            var relevantLessonResult = _relevantLessonService.GetAll().Where(y => y.TutorId == result.Id);

            List<long> lessonList = new List<long>();
            foreach (var item in relevantLessonResult)
            {
                lessonList.Add(item.LessonId);
            }
            BindLessonList(lessonList);

            var tutorUserResult = _userService.GetSingle(x => x.Id == result.UserId);
            var tutorDto = new TutorDto()
            {
                TutorName = result.TutorName,
                UserName = tutorUserResult.UserName,
                MobileNo = tutorUserResult.MobileNumber,
                Id = result.Id
            };

            var tempView = Mapper.Map<TutorDto>(tutorDto);
            return View("TutorAccount", tempView);

        }

        [HttpGet]
        public IActionResult _EditLesson(long id)
        {
            var result = _tutorService.GetSingle(x => x.Id == id);
            var tutorDto = new TutorDto();
            //var relevantLessonResult = _relevantLessonService.GetAll().Where(y => y.TutorId == result.Id);

            //List<long> lessonList = new List<long>();
            //foreach (var item in relevantLessonResult)
            //{
            //    lessonList.Add(item.LessonId);
            //}
            //BindLessonList(lessonList);
            var lessonidList = _relevantLessonService.GetAll(x => x.TutorId == result.Id).Select(x => x.LessonId).ToList();
            tutorDto = new TutorDto()
            {
                Id = id,
                RelevantLesson = string.Join(",", lessonidList)
            };
            return View(@"Components/_EditLesson", tutorDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditLesson(TutorDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    //string[] lessonIds = model.RelevantLesson.Split(",");
                    List<long> lessonIds = JsonConvert.DeserializeObject<List<long>>(model.RelevantLesson);

                    var relevantTutor = _relevantLessonService.GetAll().Where(x => x.TutorId == model.Id).ToList();
                    foreach (var item in relevantTutor)
                    {
                        await _relevantLessonService.DeleteAsync(item, Accessor, User.GetUserId());
                    }
                    foreach (var lesson in lessonIds)
                    {
                        var relevantLessonData = new RelevantLessonDto()
                        {
                            TutorId = model.Id,
                            LessonId =lesson
                        };
                        var relevantLessonObj = Mapper.Map<TutorRelevantLesson>(relevantLessonData);
                        var relevantLessonResult = await _relevantLessonService.InsertAsync(relevantLessonObj, Accessor, User.GetUserId());
                    }
                    //StaffLog
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateTutorRelevantLesson }, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateTutorRelevantLesson);
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
            var tutorDto = new TutorDto()
            {
                Id = id
            };
            return View(@"Components/_EditPassword", tutorDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditPassword(TutorDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _tutorService.GetSingle(x => x.Id == model.Id);
                    var userResult = _userService.GetSingle(x => x.Id == result.UserId);
                    userResult.PasswordHash = model.TutorPassword;
                    var updateResult = await _userService.UpdateAsync(userResult, Accessor, User.GetUserId());
                    if (updateResult != null)
                    {
                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateTutorPassword }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateTutorPassword);
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
                    ErrorLog.AddErrorLog(ex, "EditStudentPassword");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetCountTutorAnswerLessionWise(JQueryDataTableParamModel param, TutorFilterLessionWiseAnswerDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(2, new SqlParameter("@ID", SqlDbType.Int) { Value = model.Tid });



                    var allList = await _tutorService.GetFilterTutorReport(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetFilterTutorReport");
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
        public async Task<IActionResult> GetTutorList(JQueryDataTableParamModel param)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    var allList = await _tutorService.GetTutorList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetTutorList");
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
        public async Task<IActionResult> GetCountWiseTutorReport(JQueryDataTableParamModel param, TutorFilterLessionWiseAnswerDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(2, new SqlParameter("@TutorId", SqlDbType.Int) { Value = model.Tid });



                    var allList = await _tutorService.GetCountWiseTutorReport(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetFilterTutorReport");
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


        [HttpPost]
        public async Task<IActionResult> ManageTutorIsActive(long id)
        {
            try
            {
                var tutorObj = _tutorService.GetSingle(x => x.Id == id);
                tutorObj.IsActive = !tutorObj.IsActive;
                var updateResult = await _tutorService.UpdateAsync(tutorObj, Accessor, User.GetUserId());
                if (updateResult != null)
                {
                    //StaffLog
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = $@"Tutor {(tutorObj.IsActive ? "activated" : "deactivated")} successfully." }, Accessor, User.GetUserId());
                    return JsonResponse.GenerateJsonResult(1, $@"Tutor {(tutorObj.IsActive ? "activated" : "deactivated")} successfully.");
                }
                else
                {
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-/Tutor/ManageTutorIsActive");
                return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Removetutors(long[] id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var item in id)
                    {
                        var tutorObj = _tutorService.GetSingle(x => x.Id == item);
                        if (tutorObj.Id != 0)
                        {
                            tutorObj.IsDelete = true;
                            await _tutorService.UpdateAsync(tutorObj, Accessor, User.GetUserId());
                            _tutorService.Save();
                            var relevantObj = _relevantLessonService.GetAll().Where(x => x.TutorId == item);
                            foreach (var deleteObj in relevantObj)
                            {
                                await _relevantLessonService.DeleteAsync(deleteObj, Accessor, User.GetUserId());
                                await _relevantLessonService.SaveAsync();
                            }
                        }
                    }

                    //StaffLog
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.DeleteTutor }, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.DeleteTutor);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-Tutor Delete");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        public IActionResult _AddEditTutor(long id)
        {
            if (id == 0)
            {
                BindLessonList(null);
                return base.View(@"Components/_AddEditTutor", new TutorDto { Id = id });
            }
            var result = _tutorService.GetSingle(x => x.Id == id);
            var relevantLessonResult = _relevantLessonService.GetAll().Where(y => y.TutorId == result.Id);

            List<long> lessonList = new List<long>();
            foreach (var item in relevantLessonResult)
            {
                lessonList.Add(item.LessonId);
            }
            BindLessonList(lessonList);

            var tutorUserResult = _userService.GetSingle(x => x.Id == result.UserId);
            var tutorDto = new TutorDto()
            {
                DateofEmployeement = result.CreatedDate,
                //LessonId = result.LessonId,
                TutorName = result.TutorName,
                UserName = tutorUserResult.UserName,
                MobileNo = tutorUserResult.MobileNumber,
                Id = result.Id
            };

            var tempView = Mapper.Map<TutorDto>(tutorDto);
            return View(@"Components/_AddEditTutor", tempView);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditTutor(TutorDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        ErrorLog.AddErrorLog(new Exception("ModelStateIsValid"), "CreateTutor");
                        return RedirectToAction(@"Components/_AddEditTutor", model.Id);
                    }

                    if (model.Id == 0)
                    {
                        if (!string.IsNullOrEmpty(model.UserName))
                        {
                            bool userExist = UserNameExists(model.UserName);
                            if (userExist == true)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(3, ResponseConstants.TutorUserExist);
                            }
                        }
                        if (!string.IsNullOrEmpty(model.MobileNo))
                        {
                            bool mobileNoExist = MobileNoExists(model.MobileNo);
                            if (mobileNoExist == true)
                            {
                                txscope.Dispose();
                                return JsonResponse.GenerateJsonResult(3, ResponseConstants.MobileNoExist);
                            }
                        }
                        //Created by company Tutor
                        ApplicationUser tutorUser = new ApplicationUser();
                        tutorUser.UserName = model.UserName;
                        tutorUser.MobileNumber = model.MobileNo;
                        tutorUser.IsActive = true;
                        var createStudentUser = await _userManager.CreateAsync(tutorUser, model.TutorPassword);

                        if (createStudentUser.Succeeded)
                        {
                            var isRoleAssined = await _userManager.AddToRoleAsync(tutorUser, UserRoles.Tutor);

                            var tutorObj = Mapper.Map<Tutors>(model);
                            tutorObj.CreatedDate = model.DateofEmployeement;
                            tutorObj.IsActive = true;
                            tutorObj.UserId = tutorUser.Id;
                            var result = await _tutorService.InsertAsync(tutorObj, Accessor, User.GetUserId());
                            if (result != null)
                            {
                                List<long> lessonIds = JsonConvert.DeserializeObject<List<long>>(model.RelevantLesson);
                                // string[] lessonIds = model.RelevantLesson.Split(",");
                                foreach (var lesson in lessonIds)
                                {
                                    var relevantLessonData = new RelevantLessonDto()
                                    {
                                        TutorId = result.Id,
                                        LessonId = lesson
                                    };
                                    var relevantLessonObj = Mapper.Map<TutorRelevantLesson>(relevantLessonData);
                                    var relevantLessonResult = await _relevantLessonService.InsertAsync(relevantLessonObj, Accessor, User.GetUserId());
                                }

                                //Staff Log
                                if (User.IsInRole(UserRoles.Staff))
                                    await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewTutor }, Accessor, User.GetUserId());
                                txscope.Complete();
                                return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateNewTutor);
                            }
                        }

                    }

                    else if (model != null)
                    {
                        var result = await _tutorService.GetSingleAsync(x => x.Id == model.Id);
                        result.TutorName = model.TutorName;
                        await _tutorService.UpdateAsync(result, Accessor, User.GetUserId());
                        var tutorUserObj = await _userService.GetSingleAsync(x => x.Id == model.Id);
                        tutorUserObj.UserName = model.UserName;
                        tutorUserObj.MobileNumber = model.MobileNo;
                        await _userService.UpdateAsync(tutorUserObj, Accessor, User.GetUserId());
                        string[] lessonIds = model.RelevantLesson.Split(",");
                        var relevantTutor = _relevantLessonService.GetAll().Where(x => x.TutorId == model.Id).ToList();
                        foreach (var item in relevantTutor)
                        {
                            await _relevantLessonService.DeleteAsync(item, Accessor, User.GetUserId());
                        }
                        foreach (string lesson in lessonIds)
                        {
                            var relevantLessonData = new RelevantLessonDto()
                            {
                                TutorId = result.Id,
                                LessonId = Convert.ToInt64(lesson)
                            };
                            var relevantLessonObj = Mapper.Map<TutorRelevantLesson>(relevantLessonData);
                            var relevantLessonResult = await _relevantLessonService.InsertAsync(relevantLessonObj, Accessor, User.GetUserId());
                        }
                        //Staff Log
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateTutor }, Accessor, User.GetUserId());

                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateTutor);
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
                    ErrorLog.AddErrorLog(ex, "CreateTutor");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        #region DropDown

        public void BindGradeList(long id)
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

                ViewBag.GradeList = _gradeService.GetAll().Where(a => a.IsDelete == false).Select(a =>
                                        new SelectListItem()
                                        {
                                            Selected = long.Equals(a.Id, id),

                                            Value = a.Id.ToString(),
                                            Text = a.Name,
                                        }).OrderBy(a => a.Text).ToList();
            }
        }

        public void BindLessonList(List<long> lessonIds)
        {
            if (lessonIds != null)
            {
                List<SelectListItem> lstLessonItems = new List<SelectListItem>();

                foreach (var item in lessonIds)
                {
                    var lstSelectedLesson = new List<SelectListItem>();

                    lstSelectedLesson = _lessonService.GetAll().Where(x => x.Id == item && x.IsDelete == false).Select(a =>
                                                  new SelectListItem()
                                                  {
                                                      Value = a.Id.ToString(),
                                                      Text = a.Name,
                                                      Selected = true
                                                  }).OrderBy(a => a.Text).ToList();
                    foreach (var i in lstSelectedLesson)
                    {
                        var data = lstLessonItems.Where(x => x.Value == i.Value).Count();
                        if (data == 0)
                            lstLessonItems.Add(i);
                    }
                }
                foreach (var item in lessonIds)
                {
                    var lstUnSelectedLesson = new List<SelectListItem>();
                    lstUnSelectedLesson = _lessonService.GetAll().Where(x => x.Id != item && x.IsDelete == false).Select(a =>
                                              new SelectListItem()
                                              {
                                                  Value = a.Id.ToString(),
                                                  Text = a.Name,
                                                  Selected = false
                                              }).OrderBy(a => a.Text).ToList();
                    foreach (var i in lstUnSelectedLesson)
                    {
                        var data = lstLessonItems.Where(x => x.Value == i.Value).Count();
                        if (data == 0)
                            lstLessonItems.Add(i);
                    }
                }
                ViewBag.LessonList = lstLessonItems;
            }
            else
            {
                ViewBag.LessonList = _lessonService.GetAll().Where(a => a.IsDelete == false).Select(a =>
                                          new SelectListItem()
                                          {
                                              Value = a.Id.ToString(),
                                              Text = a.Name
                                          }).OrderBy(a => a.Text).ToList();
            }

        }

        public JsonResult GetLessonByGrade(int gradeId)
        {
            List<SelectListItem> lessonlist = new List<SelectListItem>();
            // ------- Getting Data from Database Using EntityFrameworkCore -------
            lessonlist = _lessonService.GetAll().Where(a => a.IsDelete == false && a.Id == gradeId).Select(a =>
                                            new SelectListItem()
                                            {
                                                Value = a.Id.ToString(),
                                                Text = a.Name
                                            }).OrderBy(a => a.Text).ToList();

            // ------- Inserting Select Item in List -------
            //lessonlist.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
            return Json(new SelectList(lessonlist, "Value", "Text"));
        }

        //public JsonResult LessonByGrade()
        //{
        //    List<SelectListItem> lessonlist = new List<SelectListItem>();
        //    // ------- Getting Data from Database Using EntityFrameworkCore -------
        //    lessonlist = _lessonService.GetAll().Where(x=>x.IsDelete==false).Select(y =>
        //                                    new SelectListItem()
        //                                    {
        //                                        Value = y.Id.ToString(),
        //                                        Text = y.Name
        //                                    }).OrderBy(a => a.Text).ToList();

        //    // ------- Inserting Select Item in List -------
        //    //lessonlist.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
        //    return Json(new SelectList(lessonlist, "Value", "Text"));
        //}



        #endregion

        #region Duplicate Mobile no and Username
        public bool UserNameExists(string userName)
        {
            var userNameExist = _userService.GetSingle(x => x.UserName == userName);
            return userNameExist != null ? true : false;
        }
        public bool MobileNoExists(string mobileNo)
        {
            var userMobileNoData = _userService.GetSingle(x => x.MobileNumber.Equals(mobileNo));
            return userMobileNoData != null ? true : false;
        }
        #endregion

        #endregion

    }
}