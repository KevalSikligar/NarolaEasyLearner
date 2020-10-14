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
using EasyLearnerAdmin.Utility;
using EasyLearnerAdmin.Utility.Common;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class LessonController : BaseController<LessonController>
    {
        #region Fields
        private readonly ILessonService _lessonService;
        private readonly ILogService _staffLog;
        #endregion

        #region Ctor
        public LessonController(ILessonService lessonService, ILogService staffLog)
        {
            _lessonService = lessonService;
            _staffLog = staffLog;

        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Lesson
        [HttpGet]
        public IActionResult LessonDetail()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetLessonist(JQueryDataTableParamModel param,int GradeId)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                    var searchRecords = new SqlParameter { ParameterName = "@SearchRecords", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

                    if (GradeId != 0)
                    {
                        parameters.Parameters.Insert(0, new SqlParameter("@GradeId", SqlDbType.BigInt) { Value = GradeId });
                    }
                        var allList = await _lessonService.GetLessonList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetLessonList");
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
        public IActionResult AddEditLesson(long id)
        {
            if (id == 0)
            {
                return View("AddEditGrade", new GradeDto { Id = id });
            }
            var result = _lessonService.GetSingle(x => x.Id == id);
            var tempView = Mapper.Map<GradeDto>(result);
            return View(@"AddEditLesson", tempView);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> AddEditGrade(LessonDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        //return RedirectToAction("AddEditGrade", model.Id);
                        RedirectToAction("AddEditLesson", model.Id);
                    }

                    if (model.Id == 0)
                    {
                        var lessonObj = Mapper.Map<Lessons>(model);
                        lessonObj.IsActive = true;
                        var result = await _lessonService.InsertAsync(lessonObj, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewLesson }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return Json(new { success = true, responseText = ResponseConstants.CreateNewLesson });
                        }

                    }
                    else if (model != null)
                    {
                        var result = await _lessonService.GetSingleAsync(x => x.Id == model.Id);
                        result.Name = model.Name;
                        await _lessonService.UpdateAsync(result, Accessor, User.GetUserId());

                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateLesson }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return Json(ResponseConstants.UpdateLesson);
                    }
                    else
                    {
                        txscope.Dispose();
                        return Json(ResponseConstants.SomethingWrong);
                    }

                    txscope.Dispose();
                    return Json(ResponseConstants.SomethingWrong);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "CreateLesson");
                    return Json(ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteLesson(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var category = _lessonService.GetSingle(x => x.Id == id);
                    category.IsDelete = true;
                    await _lessonService.UpdateAsync(category, Accessor, User.GetUserId());
                    //StaffLog
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.DeleteGrade }, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.DeleteGrade);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Get-DeleteLesson");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        

        #endregion

    }
}