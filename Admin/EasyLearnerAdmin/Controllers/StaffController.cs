using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Enums;
using EasyLearner.Service.Exception;
using EasyLearner.Service.GlobalConstant;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Models;
using EasyLearnerAdmin.Utility.Common;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class StaffController : BaseController<StaffController>
    {
        #region Fields
        private readonly IStaffService _staff;
        private readonly IStaffAccessService _staffAccess;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMenuAccessService _menu;
        private readonly ILogService _staffLog;
        private readonly IUserService _user;
        

        #endregion

        #region Constructor
        public StaffController(IUserService user, ILogService staffLog, IStaffService staffService, IStaffAccessService staffAccessService, UserManager<ApplicationUser> userManager, IMenuAccessService menuAccessService)
        {
            _user = user;
            _staffLog = staffLog;
            _staff = staffService;
            _staffAccess = staffAccessService;
            _userManager = userManager;
            _menu = menuAccessService;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(string UserName, bool isEdit = false)
        {
            try
            {
                if (String.IsNullOrEmpty(UserName)) { return View(new StaffDto { isEdit = false }); }
                else
                {
                    StaffDto staffAccess = new StaffDto();
                    var staffResult = await _userManager.FindByNameAsync(UserName);
                    var staff = await _staff.GetSingleAsync(x => x.UserId == staffResult.Id);
                    staffAccess.UserName = staffResult.UserName;
                    staffAccess.IsActive = staffResult.IsActive;
                    staffAccess.JobTitle = staff.JobTitle;
                    staffAccess.Name = staffResult.FirstName;
                    staffAccess.Mobile = staffResult.MobileNumber;
                    staffAccess.DateOfEmployment = staff.CreatedDate.ToString();
                    staffAccess.Id = staff.Id;
                    staffAccess.isEdit = isEdit;
                    staffAccess.UserId = staffResult.Id;
                    staffAccess.IsActive = staffResult.IsActive;

                    if (staff != null)
                    {
                        var staffAccessList = _staffAccess.GetAll(x => x.StaffId == staff.Id).Select(x => x.MenuId);
                        var menuList = _menu.GetAll(x => staffAccessList.Contains(x.Id)).Select(x => x.MenuName);
                        foreach (var menu in menuList)
                        {
                            switch (menu)
                            {
                                case MenuAccessConstant.Student: staffAccess.Student = true; break;
                                case MenuAccessConstant.Exams: staffAccess.Exams = true; break;
                                case MenuAccessConstant.Financial: staffAccess.Financial = true; break;
                                case MenuAccessConstant.OutBox: staffAccess.OutBox = true; break;
                                case MenuAccessConstant.QA: staffAccess.QA = true; break;
                                case MenuAccessConstant.Setting: staffAccess.Setting = true; break;
                                case MenuAccessConstant.Support: staffAccess.Support = true; break;
                                case MenuAccessConstant.Tutor: staffAccess.Tutor = true; break;
                                default: break;

                            }

                        }

                    }
                    return View(staffAccess);
                }
            }
            catch (Exception e)
            {

                return View();
            }
        }

        [HttpGet]
        public IActionResult EditStaff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddorRemoveStaff(StaffDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    var appUser = _userManager.FindByNameAsync(model.UserName).Result;
                    long staffId = 0;
                    if (appUser != null)
                    {
                        appUser.UserName = model.UserName;
                        appUser.FirstName = model.Name;
                        appUser.MobileNumber = model.Mobile;
                        var updateResult = await _userManager.UpdateAsync(appUser);
                        if (updateResult != null)
                        {
    
                            // await _userManager.RemovePasswordAsync(appUser);
                            // await _userManager.AddPasswordAsync(appUser, model.Password);
                            var staff = _staff.GetSingle(x => x.UserId == appUser.Id);
                            staff.JobTitle = model.JobTitle;
                            staff.CreatedDate = Convert.ToDateTime(model.DateOfEmployment);
                            
                            var staffResult = await _staff.UpdateAsync(staff, Accessor, User.GetUserId());
                            await ManageMenuAccess(model, staffResult.Id);
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = "Staff Updated" }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, "Staff Updated");
                        }
                        else
                        {
                            txscope.Dispose();
                            ErrorLog.AddErrorLog(null, "Error in Update Staff");
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                        }

                    }
                    else
                    {

                        #region CreateStaff

                        ApplicationUser user = new ApplicationUser();
                        user.MobileNumber = model.Mobile;
                        user.FirstName = model.Name;
                        user.UserName = model.UserName;
                        
                        user.IsActive = true;
                        var userResult = await _userManager.CreateAsync(user, model.Password);

                        if (userResult != null)
                        {
                            var staffObj = new Staff();
                            staffObj.JobTitle = model.JobTitle;
                            staffObj.UserId = user.Id;
                            staffObj.IsActive = true;
                            staffObj.CreatedDate = Convert.ToDateTime(model.DateOfEmployment);
                            var staffResult = await _staff.InsertAsync(staffObj, Accessor, User.GetUserId());
                            staffId = staffResult.Id;
                            await ManageMenuAccess(model, staffId);

                            //Assign Staff Role
                            await _userManager.AddToRoleAsync(user, UserRoles.Staff);
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow,StaffId = User.GetUserId(), Description = "New Staff Created" }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, "New Staff Created");
                        }
                        else
                        {
                            txscope.Dispose();
                            ErrorLog.AddErrorLog(null, "Error in Create Staff");
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                        }


                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "AddorRemoveStaff");
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                }
            }


        }


        public IActionResult StaffLog()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffList(JQueryDataTableParamModel param, StaffDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.StartDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.EndDate).ToString("yyyy/MM/dd") });
                    var allList = await _staffLog.GetStaffList(parameters.Parameters.ToArray());
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
                    ErrorLog.AddErrorLog(ex, "GetStaffList");
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
        public async Task<IActionResult> DeleteStaff(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var staffUser = _userManager.FindByIdAsync(id.ToString()).Result;
                    staffUser.IsActive = !staffUser.IsActive;
                    var UpdateResult = await _userManager.UpdateAsync(staffUser);
                   // var removeRoleResult = await _userManager.RemoveFromRoleAsync(staffUser, UserRoles.Staff);
                    if (UpdateResult.Succeeded)
                    {
                        var staff = _staff.GetSingle(x => x.UserId == id);
                        staff.IsActive = !staff.IsActive;
                       var staffUpdate= await _staff.UpdateAsync(staff, Accessor, User.GetUserId());

                        if (staffUpdate != null)
                        {
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate = DateTime.UtcNow, StaffId = User.GetUserId(), Description = staffUser.IsActive ? ResponseConstants.ActivatedStaff : ResponseConstants.DeactivatedStaff }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, staffUser.IsActive? ResponseConstants.ActivatedStaff : ResponseConstants.DeactivatedStaff);
                        }

                    }
                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.StaffDeletionError);

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-DeleteStaff");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpGet]    
        public IActionResult CreateReport(StaffReportDto model)
        {
               
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetStaffOperationReport(JQueryDataTableParamModel param, StaffReportDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                    parameters.Parameters.Insert(0, new SqlParameter("@StaffIdList", SqlDbType.VarChar) { Value = model.staffId });
                    parameters.Parameters.Insert(1, new SqlParameter("@Fromdate", SqlDbType.VarChar) { Value = Convert.ToDateTime(model.StartDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(2, new SqlParameter("@Todate", SqlDbType.VarChar) { Value = Convert.ToDateTime(model.EndDate).ToString("yyyy/MM/dd") });
                    var allList = await _staffLog.GetStaffOperationReport(parameters.Parameters.ToArray());
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
                    ErrorLog.AddErrorLog(ex, "GetStaffOperationReport");
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



        #endregion

        #region Common

        public async Task ManageMenuAccess(StaffDto model, long staffId)
        {

            if (model.Setting)
            {
                long menuId = 0;
                var settingResult = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Setting)).FirstOrDefault();
                if (settingResult == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.Setting }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = settingResult.Id;
                }
                var staffSeting = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffSeting == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }

            }

            if (model.Student)
            {
                long menuId = 0;
                var StudentResult = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Student)).FirstOrDefault();
                if (StudentResult == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.Student }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = StudentResult.Id;
                }
                var staffStudent = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffStudent == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }
            }
            if (model.Tutor)
            {
                long menuId = 0;
                var TutorResult = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Tutor)).FirstOrDefault();
                if (TutorResult == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.Tutor }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = TutorResult.Id;
                }
                var staffTutor = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffTutor == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }

            }
            if (model.QA)
            {
                long menuId = 0;
                var settingQA = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.QA)).FirstOrDefault();
                if (settingQA == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.QA }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = settingQA.Id;
                }
                var staffQA = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffQA == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }
            }
            if (model.Support)
            {
                long menuId = 0;
                var settingSupport = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Support)).FirstOrDefault();
                if (settingSupport == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.Support }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = settingSupport.Id;
                }
                var staffSupport = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffSupport == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }
            }
            if (model.OutBox)
            {
                long menuId = 0;
                var settingOutBox = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.OutBox)).FirstOrDefault();
                if (settingOutBox == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.OutBox }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = settingOutBox.Id;
                }
                var staffOutBox = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffOutBox == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }
            }
            if (model.Financial)
            {
                long menuId = 0;
                var settingFinancial = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Financial)).FirstOrDefault();
                if (settingFinancial == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.Financial }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = settingFinancial.Id;
                }
                var staffFinancial = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffFinancial == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }
            }
            if (model.Exams)
            {
                long menuId = 0;
                var settingExams = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Exams)).FirstOrDefault();
                if (settingExams == null)
                {
                    var insertResult = await _menu.InsertAsync(new MenuAccess { MenuName = MenuAccessConstant.Exams }, Accessor, User.GetUserId());
                    menuId = insertResult.Id;

                }
                else
                {
                    menuId = settingExams.Id;
                }
                var staffFinancial = _staffAccess.GetAll(x => x.StaffId == staffId && x.MenuId == menuId).FirstOrDefault();
                if (staffFinancial == null)
                {
                    var finalAccessResult = await _staffAccess.InsertAsync(new StaffAccess { MenuId = menuId, StaffId = staffId }, Accessor, User.GetUserId());
                }
            }

            //removing access
            if (!model.Setting)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Setting)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }
            if (!model.Student)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Student)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }
            if (!model.Tutor)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Tutor)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }
            if (!model.QA)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.QA)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }
            if (!model.Support)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Support)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }
            if (!model.OutBox)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.OutBox)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }
            if (!model.Financial)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Financial)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }
            if (!model.Exams)
            {
                var obj = _menu.GetAll(x => x.MenuName.Equals(MenuAccessConstant.Exams)).Select(x => x.Id);
                var delObj = _staffAccess.GetAll(x => x.StaffId == staffId && obj.Contains(x.MenuId));
                _staffAccess.DeleteRange(delObj);
            }


        }

        public async Task<bool> CheckUserName(string UserName)
        {
            var result = await _userManager.FindByNameAsync(UserName);
            return result == null ? false : true;
        }

        public async Task<bool> CheckUserNameForNewStaff(string UserName)
        {
            var result = await _userManager.FindByNameAsync(UserName);
            return result == null ? true : false;
        }
        public bool CheckMobileForNewStaff(string Mobile)
        {
            var result =  _user.GetSingle(x=>x.MobileNumber.Equals(Mobile));
            return result == null ? true : false;
        }


        #endregion

    }
}