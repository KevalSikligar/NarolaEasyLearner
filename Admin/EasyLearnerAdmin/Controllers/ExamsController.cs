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
using Newtonsoft.Json;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class ExamsController : BaseController<ExamsController>
    {
        private readonly IExamService _examService;
        private readonly IExamsQuestionService _examQuestionService;
        private readonly ILogService _staffLog;
        private readonly IExamLessonService _examLessonService;

        #region Fields
        #endregion

        #region Constructor
        public ExamsController(IExamLessonService examLessonService, ILogService staffLog, IExamService examService, IExamsQuestionService examQuestionService)
        {
            _staffLog = staffLog;
            _examService = examService;
            _examQuestionService = examQuestionService;
            _examLessonService = examLessonService;
        }
        #endregion

        #region Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetExamList(JQueryDataTableParamModel param)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    var allList = await _examService.GetExamList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetExamList");
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
        public IActionResult _AddExam(long id)
        {
            if (id == 0) return View(@"Components/_AddExam", new ExamDto { Id = id });
            var result = _examService.GetSingle(x => x.Id == id);
            var tempView = Mapper.Map<ExamDto>(result);
            return View(@"Components/_AddExam", tempView);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExam(ExamDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("AddExam", model.Id);
                    }

                    if (model.Id == 0)
                    {
                        model.CreatedDate = DateTime.UtcNow;
                        var examObj = Mapper.Map<Exams>(model);
                        examObj.IsActive = true;
                        var result = await _examService.InsertAsync(examObj, Accessor, User.GetUserId());
                        if (result != null)
                        {

                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewExam }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateNewExam);
                        }

                    }
                    else if (model != null)
                    {
                        var result = await _examService.GetSingleAsync(x => x.Id == model.Id);

                        result.ExamNameAndYear = model.ExamNameAndYear;
                        await _examService.UpdateAsync(result, Accessor, User.GetUserId());

                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.Now ,StaffId = User.GetUserId(), Description = ResponseConstants.UpdateExam }, Accessor, User.GetUserId());

                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateExam);
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
                    ErrorLog.AddErrorLog(ex, "CreateExam");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpGet]
        public IActionResult _AddQuestion(long id, long? q = 0)
        {
            var ExamObj = _examService.GetSingle(x => x.Id == id);
            var tempView = new ExamDto();
            if (ExamObj != null)
            {

                tempView.ExamId = ExamObj.Id;
                tempView.ExamNameAndYear = ExamObj.ExamNameAndYear;
            
                tempView.Id = 0;
            }
            if (q != 0)
            {
                var questionResult = _examQuestionService.GetSingle(x => x.Id == q);
                tempView.Question = questionResult.Question;
                tempView.Answer = questionResult.Answer;
                var lessonidList = _examLessonService.GetAll(x => x.ExamQuestionId == questionResult.Id).Select(x => x.LessonId).ToList();
                tempView.LessonIdList = string.Join(",", lessonidList);
                tempView.QuestionId = questionResult.Id;
                tempView.FileName = questionResult.FileName;
            }


            return View(@"Components/_AddQuestion", tempView);
        }

       

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion([FromForm] ExamDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("AddQuestion", model.Id);
                    }
                    #region LessonFile
                    string newExamFile = string.Empty, newLicenseFile = string.Empty;
                    if (model.ExamFile != null)
                    {
                        newExamFile = $@"Exam-{CommonMethod.GetFileName(model.ExamFile.FileName)}";
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathListConstant.ExamFiles, newExamFile, model.ExamFile);

                    }
                    #endregion

                    if (model.QuestionId != 0) {
                        var examQuestionObj = _examQuestionService.GetSingle(x => x.Id == model.QuestionId);
                        examQuestionObj.Question = model.Question;    
                        examQuestionObj.Answer = model.Answer;    
                        examQuestionObj.FileName = newExamFile;
                        var questionUpdateResult = await _examQuestionService.UpdateAsync(examQuestionObj, Accessor, User.GetUserId());
                        if (questionUpdateResult != null)
                        {
                            var ExamLessonList = _examLessonService.GetAll(x => x.ExamQuestionId == questionUpdateResult.Id);
                            _examLessonService.DeleteRange(ExamLessonList);
                            _examLessonService.Save();
                            List<long> lessonIdList = JsonConvert.DeserializeObject<List<long>>(model.LessonIdList);
                            foreach (var lessonid in lessonIdList)
                            {
                                var ExamLesson = new ExamLessons();
                                ExamLesson.ExamQuestionId = questionUpdateResult.Id;
                                ExamLesson.LessonId = lessonid;
                                var examLessonResult = await _examLessonService.InsertAsync(ExamLesson, Accessor, User.GetUserId());
                            }
                            
                        }
                        //StaffLog  
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.Now ,StaffId = User.GetUserId(), Description = ResponseConstants.UpdateExamQuestion }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateExamQuestion);

                    } else { 

                    model.CreatedDate = DateTime.UtcNow;
                    ExamsQuestions eq = new ExamsQuestions();
                    eq.ExamId = model.ExamId;
                    eq.IsActive = true;
                    eq.Question = model.Question;
                    eq.Answer = model.Answer;
                    eq.FileName = newExamFile;
                    var result = await _examQuestionService.InsertAsync(eq, Accessor, User.GetUserId());
                    if (result != null)
                    {


                        //Adding Lessons to the Exams Questions
                        List<long> lessonList = JsonConvert.DeserializeObject<List<long>>(model.LessonIdList);

                        foreach (var lessonid in lessonList)
                        {

                            var ExamLesson = new ExamLessons();
                            ExamLesson.ExamQuestionId = result.Id;
                            ExamLesson.LessonId = lessonid;
                            var examLessonResult = await _examLessonService.InsertAsync(ExamLesson, Accessor, User.GetUserId());

                        }

                        //StaffLog  
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateNewExamQuestion }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateNewExamQuestion);
                    }




                    txscope.Dispose();
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                    }

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "AddQuestion");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        public IActionResult EditQuestion(long id)
        {
            var tempView = _examService.GetSingle(x => x.Id == id && x.IsDelete == false);
            var examDto = Mapper.Map<ExamDto>(tempView);

            return View(examDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestionList(JQueryDataTableParamModel param, long ExamId)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));
                    parameters.Parameters.Insert(0, new SqlParameter("@ExamId", SqlDbType.BigInt) { Value = ExamId });
                    var allList = await _examService.GetQuestionList(parameters.Parameters.ToArray());
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
                    ErrorLog.AddErrorLog(ex, "GetQuestionList");
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
        public async Task<IActionResult> RemoveExam(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    var examObj = _examService.GetSingle(x => x.Id == id);
                    if (examObj.Id != 0)
                    {
                        //removing questions for this  exam

                        var QuestionObjList = _examQuestionService.GetAll(x => x.ExamId == examObj.Id);
                        foreach (var question in QuestionObjList)
                        {

                            question.IsDelete = true;
                            await _examQuestionService.UpdateAsync(question, Accessor, User.GetUserId());
                        }

                        examObj.IsDelete = true;
                        await _examService.UpdateAsync(examObj, Accessor, User.GetUserId());
                        _examService.Save();
                    }
                    //StaffLog  
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.Now ,StaffId = User.GetUserId(), Description = ResponseConstants.DeleteExam }, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.DeleteExam);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-RemoveExam");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveExamQuestion(long QuestionId)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    var questionObj = _examQuestionService.GetSingle(x => x.Id == QuestionId);
                    questionObj.IsDelete = true;
                    await _examQuestionService.UpdateAsync(questionObj, Accessor, User.GetUserId());


                    //StaffLog  
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.Now ,StaffId = User.GetUserId(), Description = ResponseConstants.DeleteExamQuestion }, Accessor, User.GetUserId());
                    txscope.Complete();

                    return JsonResponse.GenerateJsonResult(1, ResponseConstants.DeleteExamQuestion);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-RemoveExamQuestion");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }



        #endregion

        #region Common
        #endregion

    }
}