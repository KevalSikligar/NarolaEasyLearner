using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Utility.Common;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Transactions;
using EasyLearner.Service.Exception;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Utility;
using EasyLearner.Service.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using EasyLearnerAdmin.Models;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.Data.SqlClient;
using System.Data;
using EasyLearnerAdmin.Data.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EasyLearnerAdmin.Controllers
{


    [Authorize]
    public class FinancialController : BaseController<FinancialController>
    {
        #region Fields
        private readonly IMembershipService _membershipService;
        private readonly ITutorService _tutorService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        private readonly IStaffService _staffService;
        private readonly ILogService _staffLog;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Ctor
        public FinancialController(IMembershipService membershipService,
            ITutorService tutorService,
            IUserService userService,
            ILogService staffLog,
            IPaymentService paymentService,
            IStaffService staffService, ISubscriptionTypeService subscriptionTypeService, UserManager<ApplicationUser> userManager)
        {
            _membershipService = membershipService;
            _tutorService = tutorService;
            _userService = userService;
            _staffLog = staffLog;
            _paymentService = paymentService;
            _staffService = staffService;
            _subscriptionTypeService = subscriptionTypeService;
            _userManager = userManager;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            return View();
        }
        #region Student
        [HttpGet]
        public IActionResult _CreateReport()
        {

            return View(@"Components/_CreateReport");
        }

        public IActionResult CreateReport(string FromDate, string ToDate)
        {
            FinancialDto model = new FinancialDto();
            model.ViewReportList = new List<FinancialDto>();
            try
            {
                var obj = _membershipService.GetAll().Where(x => x.CreatedDate >= Convert.ToDateTime(FromDate) && x.CreatedDate <= Convert.ToDateTime(ToDate) && x.IsActive == true && x.IsDelete == false).GroupBy(x => x.MemberShipTypeId).Select(x => new { MemberShipType = _subscriptionTypeService.GetSingle(a => a.Id == x.Key).TypeId, MembershipTypeCount = x.Count(), PriceTotal = x.Sum(y => _subscriptionTypeService.GetSingle(x => x.Id == y.MemberShipTypeId).Price) }).ToList();
                model.FromDate = Convert.ToDateTime(FromDate);
                model.ToDate = Convert.ToDateTime(ToDate);
                foreach (var item in obj)
                {
                    var data = new FinancialDto();
                    data.totalPerson = item.MembershipTypeCount;
                    data.totalAmount = item.PriceTotal;
                    data.MemberShipTypes = item.MemberShipType;
                    model.ViewReportList.Add(data);
                }

                return View("viewReportMembership", model);

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public IActionResult viewReportMembership(FinancialDto model)
        {
            return View(model);

        }
        #endregion

        #region Tutor
        public IActionResult _TutorPaymentRegister()
        {
            //BindTutorDropdown();
            return View(@"Components/_TutorPaymentRegister");
        }

        public async Task<IActionResult> TutorPaymentRegister(PaymentDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("Index");
                    }

                    if (model.Id == 0)
                    {
                        model.CreatedDate = DateTime.UtcNow;
                        var userResult = _userService.GetSingle(x=>x.UserName==model.UserName.Trim());
                        if (userResult != null)
                        {
                            var tutorResult = _tutorService.GetSingle(x => x.TutorName == model.TutorName.Trim() && x.UserId== userResult.Id);
                            if (tutorResult != null)
                            {
                                model.TutorId = tutorResult.Id;
                            }
                        }
                        
                        var PaymentObj = Mapper.Map<Payments>(model);
                        PaymentObj.IsActive = true;

                        var result = await _paymentService.InsertAsync(PaymentObj, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.SuccessPayment }, Accessor, User.GetUserId());

                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SuccessPayment);
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
                    ErrorLog.AddErrorLog(ex, "post/TutorPaymentRegister");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }

        }
        [HttpPost]
        public string GetUserNameByTutorId(long id)
        {
            var userId = _tutorService.GetById(id).UserId;
            string Username = _userService.GetById(userId).UserName;
            return Username;
        }

        public IActionResult _TutorPaymentHistory(bool AllTutor)
        {
            BindTutorDropdown();
            PaymentHistoryDto model = new PaymentHistoryDto();
            model.AllTutor = AllTutor;
            //model.FromDate = Convert.ToDateTime("");
            return View(@"Components/_TutorPaymentHistory", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorPaymentHistory(JQueryDataTableParamModel param, PaymentHistoryDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));


                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });
                    if (model.TutorName == null)
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@TutorId", SqlDbType.VarChar) { Value = "" });
                    }
                    else
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@TutorId", SqlDbType.VarChar) { Value = model.TutorName });
                    }

                    var allList = await _paymentService.GetPaymentByTutorList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "TutorPaymentHistory");
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

        //public IActionResult ViewPaymentHistory(PaymentHistoryDto model)
        //{
        //    return View(model);

        //}

        public IActionResult ViewPaymentHistory(bool AllTutor)
        {
            PaymentHistoryDto model = new PaymentHistoryDto();
            model.AllTutor = AllTutor;
            return View(@"ViewPaymentHistory", model);
        }

        #endregion

        #region Staff
        public IActionResult _StaffPaymentRegister()
        {
            BindStaffDropdown();
            return View(@"Components/_StaffPaymentRegister");
        }

        public async Task<IActionResult> StaffPaymentRegister(PaymentDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("Index");
                    }

                    if (model.Id == 0)
                    {
                        model.CreatedDate = DateTime.UtcNow;
                        var staffUserResult = _userService.GetSingle(x => x.FirstName == model.StaffName.Trim());
                        if (staffUserResult != null)
                        {
                            var staffData = _staffService.GetSingle(x => x.UserId == staffUserResult.Id);
                            if (staffData != null)
                            {
                                model.StaffId = staffData.Id;
                            }
                        }
                        var PaymentObj = Mapper.Map<Payments>(model);
                        PaymentObj.IsActive = true;

                        var result = await _paymentService.InsertAsync(PaymentObj, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.SuccessPayment }, Accessor, User.GetUserId());

                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SuccessPayment);
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
                    ErrorLog.AddErrorLog(ex, "post/StaffPaymentRegister");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }

        }
        [HttpPost]
        public string GetUserNameByStaffId(long id)
        {
            var userId = _staffService.GetById(id).UserId;
            string Username = _userService.GetById(userId).UserName;
            return Username;
        }

        public IActionResult _StaffPaymentHistory(bool AllStaff)
        {
            BindStaffDropdown();
            PaymentHistoryDto model = new PaymentHistoryDto();
            model.AllStaff = AllStaff;
            return View(@"Components/_StaffPaymentHistory", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffPaymentHistory(JQueryDataTableParamModel param, PaymentHistoryDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));


                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });

                    if (model.StaffName == null)
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@StaffId", SqlDbType.VarChar) { Value = "" });

                    }
                    else
                    {
                        parameters.Parameters.Insert(2, new SqlParameter("@StaffId", SqlDbType.VarChar) { Value = model.StaffName });
                    }

                    var allList = await _paymentService.GetPaymentByStaffList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "StaffPaymentHistory");
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

        //public IActionResult ViewStaffPaymentHistory(PaymentHistoryDto model)
        //{
        //    return View(model);
        //}

        public IActionResult ViewStaffPaymentHistory(bool AllStaff)
        {
            PaymentHistoryDto model = new PaymentHistoryDto();
            model.AllStaff = AllStaff;
            return View(@"ViewStaffPaymentHistory", model);
        }
        #endregion
        public void BindTutorDropdown()
        {
            ViewBag.TutorList = _tutorService.GetAll(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Text = x.TutorName,
                Value = x.Id.ToString()
            }).OrderBy(x => x.Text).ToList();
        }
        public void BindStaffDropdown()
        {
            ViewBag.StaffList = _staffService.GetAll(x => x.IsActive == true).Select(x => new SelectListItem
            {
                Text = _userService.GetById(x.UserId).FullName,
                Value = x.Id.ToString()
            }).OrderBy(x => x.Text).ToList();
        }
        #endregion

        //public async Task<bool> CheckTutorUserName(string TutorName)
        //{
        //    var result = await _userManager.FindByNameAsync(TutorName);
        //    bool tutorUserData = false;
        //    if (result != null)
        //    {
        //        tutorUserData = await _userManager.IsInRoleAsync(result, "Tutor");
        //    }
        //    return tutorUserData;
        //}

        //public async Task<bool> CheckStaffUserName(string StaffName)
        //{
        //    var result = await _userManager.FindByNameAsync(StaffName);
        //    bool staffUserData = false;
        //    if (result != null)
        //    {
        //        staffUserData = await _userManager.IsInRoleAsync(result, "Staff");
        //    }
        //    return staffUserData;
        //}

    }
}