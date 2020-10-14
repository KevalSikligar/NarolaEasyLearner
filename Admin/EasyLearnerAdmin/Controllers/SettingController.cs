using EasyLearner.Service.Dto;
using EasyLearner.Service.Enums;
using EasyLearner.Service.Exception;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Utility.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Transactions;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class SettingController : BaseController<SettingController>
    {
        #region Fields
        private readonly ISettingService _settingService;
        private readonly ILogService _staffLog;
        //private readonly UserManager<ApplicationUser> _userManager;
        [Obsolete]
        private IHostingEnvironment _hostingEnvironment;

        #endregion

        #region Ctor
        [Obsolete]
        public SettingController(ILogService staffLog, ISettingService settingService, IHostingEnvironment hostingEnvironment)
        {
            _settingService = settingService;
            _staffLog = staffLog;
            _hostingEnvironment = hostingEnvironment;

        }
        #endregion


        #region Methods
        [HttpGet]
        public IActionResult _SettingText()
        {
            return View(@"Components/_SettingText");
        }

        [HttpPost]
        public IActionResult SetttingPageLevel(SettingDto objsetting)
        {
            var data = _settingService.GetSingle(x => x.Title == objsetting.Title && x.IsDelete == false && x.IsActive == true);
            if (data == null)
            {
                if (objsetting.Id == 0) return base.View(@"Components/_AddSetting", new SettingDto { Id = objsetting.Id, Title = objsetting.Title });
            }
            var dto = new SettingDto()
            {
                Id = data.Id,
                Title = data.Title,
                Html = data.Html
            };
            return View(@"Components/_AddSetting", dto);
        }

        [HttpPost]
        [Obsolete]
        public string UploadFile()
        {
            string result = string.Empty;
            try
            {
                long size = 0;
                var file = Request.Form.Files;
                var filename = ContentDispositionHeaderValue
                                .Parse(file[0].ContentDisposition)
                                .FileName
                                .Trim('"');
                string FilePath = _hostingEnvironment.WebRootPath + $@"\Uploads\{ filename}";
                size += file[0].Length;
                using (FileStream fs = System.IO.File.Create(FilePath))
                {
                    file[0].CopyTo(fs);
                    fs.Flush();
                }
                result = filename;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }



        [HttpPost]
        public async Task<IActionResult> AddSetting(SettingDto model)
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
                        model.UserId = User.GetUserId();
                        model.IsActive = true;
                        var dataModel = Mapper.Map<Settings>(model);
                        var result = await _settingService.InsertAsync(dataModel, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.SettingTextCreated }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SettingTextCreated);
                        }
                        else
                        {
                            txscope.Dispose();
                            ErrorLog.AddErrorLog(null, "Post-AddSettings");
                            return JsonResponse.GenerateJsonResult(0, "Settings not added.");
                        }
                    }
                    else
                    {
                        var data = _settingService.GetSingle(x => x.Id == model.Id && x.IsDelete == false && x.IsActive == true);
                        data.Id = model.Id;
                        data.Html = model.Html;
                        data.Title = model.Title;
                        data.UserId = User.GetUserId();
                        var updateResult = await _settingService.UpdateAsync(data, Accessor, User.GetUserId());
                        if (updateResult != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.SettingTextCreated }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SettingTextUpdated);
                        }
                        else
                        {

                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.SomethingWrong);
                        }
                    }

                }

                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post-AddSettings");
                    return JsonResponse.GenerateJsonResult(0);
                }
            }
            #endregion
        }
    }
}