using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Enums;
using EasyLearner.Service.Exception;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Models;
using EasyLearnerAdmin.Utility.Common;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class SupportController : BaseController<SupportController>
    {
        #region Field
        private readonly IUserService _user;
        private readonly IStudentService _student;
        private readonly ISupportRequestService _supportRequest;
        private readonly ISupportResponseService _supportResponse;
        private readonly ILogService _staffLog;
        #endregion

        #region Constructor
        public SupportController(ILogService staffLog,IStudentService student, IUserService user, ISupportRequestService supportRequest, ISupportResponseService supportResponse)
        {
            _staffLog = staffLog;
            _student = student;
            _supportRequest = supportRequest;
            _supportResponse = supportResponse;
            _user = user;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSupportList(JQueryDataTableParamModel param)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    var allList = await _supportRequest.GetSupportReportList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetSupportList");
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
        public IActionResult _ViewSupportReport(long requestId)
        {
            if (requestId == 0) return View(@"Components/_ViewSupportReport", new SupportReportDto { RequestId = requestId });
            var requestObj = _supportRequest.GetSingle(x => x.Id == requestId);
            var tempView = new SupportReportDto();
            var studentObj = _student.GetSingle(x => x.UserId == requestObj.StudentId);
            var user = _user.GetSingle(x => x.Id == requestObj.StudentId);
            tempView.SubjectOfSupport = requestObj.Subject;
            tempView.Description = requestObj.Description;
            tempView.StudentName = studentObj.StudentName;
            tempView.StudentUserName = user.UserName;
            tempView.DateOfSend = requestObj.CreatedDate;
            tempView.TimeOfSend = requestObj.CreatedDate.ToShortTimeString();
            tempView.RequestId = requestId;

            return View(@"Components/_ViewSupportReport", tempView);
        }

        public IActionResult SendResponse(long requestId)
        {
            SupportResponseDto response = new SupportResponseDto();
            var supportObj = _supportRequest.GetSingle(x => x.Id == requestId);
            response.SubjectOfRequest = supportObj.Subject;
            response.CreatedDate = supportObj.CreatedDate;
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> AddResponse(SupportResponseDto model)
        {
          TimeSpan ts = DateTime.Now - model.CreatedDate;

          // Difference in Days.
            int differenceInDays = ts.Days; // This is in int

          // Difference in Hours.
            int differenceInHours = ts.Hours; // This is in int
          

          // Difference in Minutes.
            int differenceInMinutes = ts.Minutes; // This is in int
       
            TimeSpan duration = new TimeSpan(Convert.ToInt32(ts.TotalDays), ts.Hours, ts.Minutes);

            var responseResult=await _supportResponse.InsertAsync(new Data.DbModel.SupportResponse { RequestId = model.RequestId, StaffId = User.GetUserId(), Description = model.ResponseDescription, ResponseTime= duration,Subject=model.SubjectOfResponse }, Accessor, User.GetUserId());
            if (User.IsInRole(UserRoles.Staff))
                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow,StaffId = User.GetUserId(), Description = ResponseConstants.RespondToSupportRequest }, Accessor, User.GetUserId());
            return RedirectToAction("Index");
             
        }

        public IActionResult ViewSupportHistory(long requestId)
        {
               
            return View(requestId); 
        }

        [HttpGet]
        public async Task<IActionResult> GetSupportHistoryList(JQueryDataTableParamModel param,long RequestId)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                    parameters.Parameters.Insert(0, new SqlParameter("@RequestId", SqlDbType.BigInt) { Value = RequestId });
                    var allList = await _supportRequest.GetSupportHistoryList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetSupportHistoryList");
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
        #endregion


    }
}