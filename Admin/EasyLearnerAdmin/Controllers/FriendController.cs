using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EasyLearner.Service.Dto;
using EasyLearner.Service.Enums;
using EasyLearner.Service.Exception;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Utility;
using EasyLearnerAdmin.Utility.Common;
using EasyLearnerAdmin.Utility.JqueryDataTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class FriendController : BaseController<FriendController>
    {
        #region Fields
        private readonly IFriendsService _friendsService;
        private readonly ILogService _staffLog;
        private readonly IInvitationCodeService _invitationCodesService;
        #endregion

        #region Ctor
        public FriendController(ILogService staffLog,IFriendsService friendsService, IInvitationCodeService invitationCodesService)
        {
            _staffLog = staffLog;
            _friendsService = friendsService;
            _invitationCodesService = invitationCodesService;
        }
        #endregion

        #region Method
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendsList(Models.JQueryDataTableParamModel param)

        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

                    var allList = await _friendsService.GetFriendsList(parameters.Parameters.ToArray());

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
                    ErrorLog.AddErrorLog(ex, "GetFriendsList");
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
        public IActionResult _AddEditInvitationCode()
        {
            var result = _invitationCodesService.GetAll().ToList();
            if (result.Count > 0)
            {
                var invitationCodeModel = new InvitationCodeDto()
                {
                    NoOfFreeDays = result.FirstOrDefault().NumberOfFreeDays,
                    NoOfFreeQuestions = result.FirstOrDefault().NumberOfFreeQuestions,
                    ExpirationDays = result.FirstOrDefault().ExpirationDays,
                    Id = result.FirstOrDefault().Id
                };
                return View(@"Components/_AddEditInvitationCode", invitationCodeModel);

            }
            return View(@"Components/_AddEditInvitationCode", new InvitationCodeDto { Id = 0 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditInvitationCode(InvitationCodeDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        RedirectToAction("_AddEditInvitationCode", model.Id);
                    }

                    if (model.Id == 0)
                    {
                        var invitationCodeObj = Mapper.Map<InvitationCode>(model);
                        invitationCodeObj.IsActive = true;
                        invitationCodeObj.NumberOfFreeDays = model.NoOfFreeDays; 
                        invitationCodeObj.NumberOfFreeQuestions = model.NoOfFreeQuestions;
                        invitationCodeObj.ExpirationDays = model.ExpirationDays;

                        var result = await _invitationCodesService.InsertAsync(invitationCodeObj, Accessor, User.GetUserId());
                        if (result != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateInvitaionCode }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateInvitaionCode);
                        }

                    }
                    else if (model != null)
                    {
                        var result = await _invitationCodesService.GetSingleAsync(x => x.Id == model.Id && x.IsActive==true && x.IsDelete==false);

                        result.NumberOfFreeDays = model.NoOfFreeDays;
                        result.NumberOfFreeQuestions = model.NoOfFreeQuestions;
                        result.ExpirationDays = model.ExpirationDays;
                        result.IsActive = true;
                        await _invitationCodesService.UpdateAsync(result, Accessor, User.GetUserId());


                        //StaffLog
                        if (User.IsInRole(UserRoles.Staff))
                            await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateInvitationCode }, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateInvitationCode);
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
                    ErrorLog.AddErrorLog(ex, "CreateInvitationCodeSetUp");
                    return Json(ResponseConstants.SomethingWrong);
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> ManageFriendsIsActive(long id)
        {
            try
            {
                var FriendObj = _friendsService.GetSingle(x => x.Id == id);
                FriendObj.IsActive = !FriendObj.IsActive;
                await _friendsService.UpdateAsync(FriendObj, Accessor, User.GetUserId());
                return JsonResponse.GenerateJsonResult(1, $@"Friends {(FriendObj.IsActive ? "activated" : "deactivated")} successfully.");
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-/Friend/ManageFriendsIsActive");
                return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
            }

        }
        #endregion

    }
}