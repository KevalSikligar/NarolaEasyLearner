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
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
namespace EasyLearnerAdmin.Controllers
{
    [Authorize]
    public class SubscriptionController : BaseController<SubscriptionController>
    {
        #region Fields
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogService _staffLog;
        #endregion

        #region Ctor
        public SubscriptionController(ILogService staffLog, ISubscriptionTypeService subscriptionTypeService, ISubscriptionService subscriptionService)
        {
            _staffLog = staffLog;
            _subscriptionTypeService = subscriptionTypeService;
            _subscriptionService = subscriptionService;
        }
        #endregion

        #region Methods

        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> GetSubscriptionList(JQueryDataTableParamModel param)
        //{
        //    using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //    {
        //        try
        //        {
        //            var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0));

        //            var allList = await _subscriptionService.GetSubscriptionList(parameters.Parameters.ToArray());

        //            var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
        //            return Json(new
        //            {
        //                param.sEcho,
        //                iTotalRecords = total,
        //                iTotalDisplayRecords = total,
        //                aaData = allList
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            ErrorLog.AddErrorLog(ex, "GetSubscriptionList");
        //            return Json(new
        //            {
        //                param.sEcho,
        //                iTotalRecords = 0,
        //                iTotalDisplayRecords = 0,
        //                aaData = ""
        //            });
        //        }
        //    }
        //}

        [HttpGet]
        public IActionResult _AddSubscription(long id)
        {
            if (id == 0) return View(@"Components/_AddSubscription", new SubscriptionTypeDto { Id = id });
            return View(@"Components/_AddSubscription");
        }

        [HttpGet]
        public IActionResult _EditSubscription()
        {
            return View(@"Components/_EditSubscription");
        }

        [HttpGet]
        public IActionResult _EditSubscriptionValues(long id)
        {
            var result = _subscriptionTypeService.GetSingle(x => x.TypeId == id && x.IsActive == true && x.IsDelete == false);
            if (result != null)
            {
                var subDto = new SubscriptionTypeDto()
                {
                    TypeId = result.TypeId,
                    Id = result.Id,
                    Price = result.Price,
                    AllowedQuestion = result.AllowedQuestion,
                    AllowedDays = result.AllowedDays,
                    Resultcount = result.Id
                };
                var tempView = Mapper.Map<SubscriptionTypeDto>(subDto);
                return View(@"Components/_EditSubscriptionValues", tempView);
            }
            return View(@"Components/_EditSubscriptionValues", new SubscriptionTypeDto { Resultcount = 0 });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditSubscription(SubscriptionTypeDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return RedirectToAction("AddEditSubscription", model.Id);
                    }
                    if (model.Id == 0)
                    {
                        var result = await _subscriptionTypeService.GetSingleAsync(x => x.TypeId == model.TypeId && x.IsActive == true && x.IsDelete == false);
                        if (result != null)
                        {
                            return JsonResponse.GenerateJsonResult(3, ResponseConstants.MembershipServiceAlert);
                        }
                        else
                        {
                            var subscriptionTypeObj = Mapper.Map<SubscriptionType>(model);
                            subscriptionTypeObj.IsActive = true;
                            var resultSubscription = await _subscriptionTypeService.InsertAsync(subscriptionTypeObj, Accessor, User.GetUserId());
                            if (resultSubscription != null)
                            {
                                //StaffLog
                                if (User.IsInRole(UserRoles.Staff))
                                    await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.CreateSubscription }, Accessor, User.GetUserId());
                                txscope.Complete();
                                return JsonResponse.GenerateJsonResult(1, ResponseConstants.CreateSubscription);
                            }
                        }

                    }
                    else
                    {
                        var result = await _subscriptionTypeService.GetSingleAsync(x => x.TypeId == model.TypeId && x.IsActive == true && x.IsDelete == false);
                        result.TypeId = model.TypeId;
                        result.Price = model.Price;
                        result.AllowedDays = model.AllowedDays;
                        result.AllowedQuestion = model.AllowedQuestion;
                        result.IsActive = true;

                        var updateResult = await _subscriptionTypeService.UpdateAsync(result, Accessor, User.GetUserId());
                        if (updateResult != null)
                        {
                            //StaffLog
                            if (User.IsInRole(UserRoles.Staff))
                                await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = ResponseConstants.UpdateSubscription }, Accessor, User.GetUserId());
                            txscope.Complete();
                            return JsonResponse.GenerateJsonResult(1, ResponseConstants.UpdateSubscription);
                        }
                        else
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                        }

                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "CreateSubscripion");
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
            return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
        }

        [HttpPost]
        public async Task<IActionResult> ManageSubscriptionIsActive(long id)
        {
            try
            {
                var subscriptionObj = _subscriptionService.GetSingle(x => x.Id == id);
                subscriptionObj.IsActive = !subscriptionObj.IsActive;
                await _subscriptionService.UpdateAsync(subscriptionObj, Accessor, User.GetUserId());
                var subscriptionTypeObj = _subscriptionTypeService.GetSingle(x => x.Id == subscriptionObj.SubscriptionTypeId);
                subscriptionTypeObj.IsActive = !subscriptionTypeObj.IsActive;
                var updateResult = await _subscriptionTypeService.UpdateAsync(subscriptionTypeObj, Accessor, User.GetUserId());
                if (updateResult != null)
                {
                    //StaffLog
                    if (User.IsInRole(UserRoles.Staff))
                        await _staffLog.InsertAsync(new Log { CreatedDate=DateTime.UtcNow, StaffId = User.GetUserId(), Description = $@"Subscription {(subscriptionObj.IsActive ? "activated" : "deactivated")} successfully." }, Accessor, User.GetUserId());
                    return JsonResponse.GenerateJsonResult(1, $@"Subscription {(subscriptionObj.IsActive ? "activated" : "deactivated")} successfully.");
                }
                else
                {
                    return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "Post-/Subscription/ManageSubscriptionIsActive");
                return JsonResponse.GenerateJsonResult(0, ResponseConstants.SomethingWrong);
            }

        }
        #endregion
    }
}