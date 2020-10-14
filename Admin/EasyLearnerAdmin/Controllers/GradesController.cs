using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Models;
using EasyLearnerAdmin.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EasyLearner.Service.Exception;
using EasyLearnerAdmin.Utility.Common;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Enums;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class GradesController : BaseController<GradesController>
    {
        #region Fields
        private readonly IGradeService _gradeService;
        private readonly ILessonService _lessonService;
        private readonly ILogService _staffLog;
        #endregion

        #region Ctor
        public GradesController(ILogService staffLog,IGradeService gradeService, ILessonService lessonService)
        {
            _staffLog = staffLog;
            _gradeService = gradeService;
            _lessonService = lessonService;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetGradeList(JQueryDataTableParamModel param)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    var allList = await _gradeService.GetGradeList(parameters.Parameters.ToArray());
                    foreach (var grade in allList)
                    {
                        grade.lessonList = new List<LessonDto>();
                        var llist = Mapper.Map<List<LessonDto>>(_lessonService.GetAll(x => x.GradeId == grade.Id && x.IsDelete==false).ToList());
                        if (llist.Count > 0)
                            grade.lessonList.AddRange(llist);
                    }


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
                    ErrorLog.AddErrorLog(ex, "GetGradeList");
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
        public IActionResult _AddEditGrade(long id)
        {
            if (id == 0) return View(@"Components/_AddEditGrade", new GradeDto { Id = id });
            var result = _gradeService.GetSingle(x => x.Id == id);
            var tempView = Mapper.Map<GradeDto>(result);
            return View(@"Components/_AddEditGrade", tempView);
        }

        [HttpGet]
        public IActionResult _AddEditLesson(long g=0,long l=0)
        {
           
            if (l == 0) return View(@"Components/_AddEditLesson", new LessonDto { GradeId= g, Id = l });
            var result = _lessonService.GetSingle(x => x.Id == l);
            var tempView = Mapper.Map<LessonDto>(result);
            
            return View(@"Components/_AddEditLesson", tempView);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditGrade(GradeDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("AddEditGrade", model.Id);
                    }

                    if (model.Id == 0)
                    {
                        model.CreatedDate = DateTime.UtcNow;
                        var gradeObj = Mapper.Map<Grades>(model);
                        gradeObj.IsActive = true;
                        var result = await _gradeService.InsertAsync(gradeObj, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewGrade }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateNewGrade);
                        }

                    }
                    else if (model != null)
                    {
                        var result = await _gradeService.GetSingleAsync(x => x.Id == model.Id);

                        result.Name = model.Name;

                        await _gradeService.UpdateAsync(result, Accessor, User.GetUserId());

                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateGrade }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateGrade);
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
                    ErrorLog.AddErrorLog(ex, "CreateGrade");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditLesson(LessonDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("AddEditLesson", model.Id);
                    }
                    #region LessonFile
                    string newLessonFile = string.Empty, newLicenseFile = string.Empty;
                    if (model.LessonFile != null)
                    {
                        newLessonFile = $@"lesson-{CommonMethod.GetFileName(model.LessonFile.FileName)}";
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathListConstant.LessonFile, newLessonFile, model.LessonFile);

                    }
                    #endregion
                    if (model.Id == 0)
                    {
                        model.CreatedDate = DateTime.UtcNow;
                        model.FileName = newLessonFile;
                        var gradeObj = Mapper.Map<Lessons>(model);
                        gradeObj.IsActive = true;
                        var result = await _lessonService.InsertAsync(gradeObj, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewLesson }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateNewLesson);
                        }

                    }
                    else if (model != null)
                    {
                        var result = await _lessonService.GetSingleAsync(x => x.Id == model.Id);
                        result.Name = model.Name;
                        result.FileName = newLessonFile;
                        await _lessonService.UpdateAsync(result, Accessor, User.GetUserId());
                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateLesson }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateLesson);
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
                    ErrorLog.AddErrorLog(ex, "CreateGrade");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageGradeIsActive(long id)
        {
            try
            {
                var gradeObj = _gradeService.GetSingle(x => x.Id == id);
                gradeObj.IsActive = !gradeObj.IsActive;
                await _gradeService.UpdateAsync(gradeObj, Accessor, User.GetUserId());
                //StaffLog
                if (User.IsInRole(UserRoles.Staff))
                    await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = "Grade Status Updated." }, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"Grade {(gradeObj.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-/Grade/ManageGradeIsActive");
                return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
            }

        }

        [HttpPost]
        public async Task<IActionResult> RemovGrades(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    
                        var gradeObj = _gradeService.GetSingle(x => x.Id == id);
                        if (gradeObj.Id != 0)
                        {
                            //removing lesson for this  grade
                
                            var lessonObjList = _lessonService.GetAll(x => x.GradeId == gradeObj.Id);
                            foreach (var lesson in lessonObjList) {
                                lesson.IsDelete = true;
                                await _lessonService.UpdateAsync(lesson, Accessor, User.GetUserId());
                            }

                            gradeObj.IsDelete = true;
                            await _gradeService.UpdateAsync(gradeObj, Accessor, User.GetUserId());
                            _gradeService.Save();
                        }
                    
                    //StaffLog
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.DeleteGrade }, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.DeleteGrade);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-DeleteDistributorRequest");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemovLesson(long[] id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    foreach (var item in id)
                    {
                        var lessonObj = _lessonService.GetSingle(x => x.Id == item);
                        if (lessonObj.Id != 0)
                        {
                            lessonObj.IsDelete = true;
                            await _lessonService.UpdateAsync(lessonObj, Accessor, User.GetUserId());
                            _gradeService.Save();
                        }
                    }

                    //StaffLog
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.DeleteLesson }, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.DeleteLesson);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-RemovLesson");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }


        [HttpGet]
        public IActionResult GetLessonList()
        {
            var gradeList = Mapper.Map<List<GradeDto>>(_gradeService.GetAll(x => x.IsActive && !x.IsDelete));
            foreach (var grade in gradeList)
            {
                grade.lessonList = new List<LessonDto>();
                var llist = Mapper.Map<List<LessonDto>>(_lessonService.GetAll(x => x.GradeId == grade.Id && !x.IsDelete).ToList());
                if (llist.Count > 0)
                    grade.lessonList.AddRange(llist);
            }
            return Json(gradeList);
        }


        [HttpGet]
        public IActionResult DisplayGradeLesson() 
            {
            var GradeList =  _gradeService.GetAll(x=>x.IsDelete==false && x.IsActive==true);
            List<DisplayGradeLessonDto> gList = new List<DisplayGradeLessonDto>();
            foreach (var grade in GradeList)
            {
                DisplayGradeLessonDto gobj = new DisplayGradeLessonDto();
                gobj.GradeName = grade.Name;
                gobj.LessonName = new List<string>();
                gobj.LessonName.AddRange(_lessonService.GetAll(x => x.GradeId == grade.Id).Select(x => x.Name));
                gList.Add(gobj);
            }
            return View(gList);
        }



        #endregion

    }
}