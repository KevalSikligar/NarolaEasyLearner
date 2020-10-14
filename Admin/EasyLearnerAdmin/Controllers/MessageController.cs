using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using AutoMapper.Configuration;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Exception;
using EasyLearner.Service.Implementation;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Utility.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.AspNetCore.Identity;
using EasyLearnerAdmin.Data.DbContext;
using EasyLearner.Service.Enums;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class MessageController : BaseController<GradesController>
    {
        #region Fields
        private readonly IMessageService _messageService;
        private readonly IStudentService _studentService;
        private readonly IConfiguration _config;
        private readonly ILogService _staffLog;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region Ctor
        public MessageController(ILogService staffLog,IMessageService messageService, UserManager<ApplicationUser> userManager, IStudentService studentService, IConfiguration config, IUserService userService)
        {
            _staffLog = staffLog;
            _messageService = messageService;
            _studentService = studentService;
            _config = config;
            _userService = userService;
            _userManager = userManager;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendMessage1(long id)
        {
            return View(new MessageDto { Id = id });
        }
        public IActionResult SendMessage2(int messageNo,long id)
        {
            if (messageNo == 2)
            {
                ViewBag.TextTitle = " Send message to those who entered their mobile but did not register!";
            }
            if (messageNo == 3)
            {
                ViewBag.TextTitle = " Send message to those whose membership has expired!";
            }
            return View(new MessageDto { Id = id });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditMessage(MessageDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("Index", model.Id);
                    }

                    if (model.Id == 0)
                    {
                        model.CreatedDate = DateTime.UtcNow;
                        model.UserId = User.GetUserId();
                        var messageObj = Mapper.Map<Message>(model);
                        messageObj.IsActive = true;
                        var result = await _messageService.InsertAsync(messageObj, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //SMSApiCalling(model);
                            //StaffLog  
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateMessage }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateMessage);
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
                    ErrorLog.AddErrorLog(ex, "CreateMessage");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
        }
        public IActionResult _AssignStudent()
        {
            return View(@"Components/_AssignStudent");
        }

        #region Dropdown
        public JsonResult GetUserList()
        {
            List<SelectListItem> userlist = new List<SelectListItem>();

            // ------- Getting Data from Database Using EntityFrameworkCore -------
            userlist = _studentService.GetAll().Where(a => a.IsDelete == false).Select(a =>
                                            new SelectListItem()
                                            {
                                                Value = a.Id.ToString(),
                                                Text = _userService.GetSingle(x=>x.Id==a.UserId).UserName,
                                            }).OrderBy(a => a.Text).ToList();

            //studentlist.Insert(0, new SelectListItem { Value = "0", Text = "--Select--" });
            return Json(new SelectList(userlist, "Value", "Text"));
        }
        #endregion

        public void SMSApiCalling(MessageDto messageDto)
        {
            var AccountSID = _config.GetValue<string>("Twilio:AccountSID");
            var AuthToken = _config.GetValue<string>("Twilio:AuthToken");

            TwilioClient.Init(AccountSID, AuthToken);
            var message = MessageResource.Create(
                body: messageDto.MessageText,
                from: new Twilio.Types.PhoneNumber(""),
                to: new Twilio.Types.PhoneNumber("")
            );
        }

        public async Task<bool> CheckUserName(string UserName)
        {
            var result = await _userManager.FindByNameAsync(UserName);
            //bool studentUserData = false;
            //if (result != null)
            //{
            //    studentUserData = await _userManager.IsInRoleAsync(result, "Staff");
            //}
            return result == null ? false : true; 
        }
        #endregion

    }
}