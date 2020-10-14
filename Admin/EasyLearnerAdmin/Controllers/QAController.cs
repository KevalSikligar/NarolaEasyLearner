using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Models;
using EasyLearnerAdmin.Utility.Common;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class QAController :  BaseController<QAController>
    {
        #region Fields
        private readonly ITutorService _tutorService;
        private readonly IUserService _userService;
        private readonly IGradeService _gradeService;
        private readonly IQuestionRequestService _questionRequestService;
        private readonly IQuestionResponseService _questionResponseService;
        private readonly ILogService _staffLog;
        #endregion

        #region Ctor
        public QAController(
            ILogService staffLog,
            ITutorService tutorService,
            IUserService userService,
            IQuestionRequestService questionRequestService,
            IQuestionResponseService questionResponseService,
            IGradeService gradeService)   
        {
            _staffLog = staffLog;
            _tutorService = tutorService;
            _userService = userService;
            _questionRequestService = questionRequestService;
            _questionResponseService = questionResponseService;
            _gradeService = gradeService;

            
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            BindGradeDropdown();
            return View();
        }
        public IActionResult ViewQAReport(QADto model)
        {
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetViewQAReport(JQueryDataTableParamModel param,QADto model)
        
        {
            
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));


                    parameters.Parameters.Insert(0, new SqlParameter("@Fromdate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.FromDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(1, new SqlParameter("@Todate", SqlDbType.DateTime) { Value = Convert.ToDateTime(model.ToDate).ToString("yyyy/MM/dd") });
                    parameters.Parameters.Insert(2, new SqlParameter("@GradeId", SqlDbType.VarChar) { Value = model.GradeList });
                    parameters.Parameters.Insert(3, new SqlParameter("@AnsweredQuestion", SqlDbType.Int) { Value = model.ansSlectionType });

                    var allList = await _questionResponseService.GetQAReport(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "ViewQAReport");
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
        public IActionResult _ViewQAInfo(long q,long r)
        {
            if (q == 0 ) return View(@"Components/_AddExam", new ExamDto { QuestionId = q });
            var questionResult = _questionRequestService.GetSingle(x => x.Id == q);
            
            var tempView = new ExamDto();
            tempView.QuestionId = questionResult.Id;
            tempView.QuestionDescription = questionResult.Description;
            tempView.QuestionFileName = questionResult.File;
            tempView.SubjectName = questionResult.SubjectName;

            if (r != 0)
            {
                var responseResult = _questionResponseService.GetSingle(x => x.Id == r);
                tempView.ResponseId = responseResult.Id;
                tempView.ResponseDescription = responseResult.Response;
                tempView.ResponseFileName = responseResult.File;
            }
            return View(@"Components/_questionInfo", tempView);
        }
        #endregion

        #region Common
        public void BindGradeDropdown()
        {
            ViewBag.GradeList = _gradeService.GetAll(x => x.IsActive == true && x.IsDelete==false).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).OrderBy(x => x.Text).ToList();
        }
        #endregion
    }
}