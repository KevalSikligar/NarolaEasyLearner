using AutoMapper;
using AutoMapper.Configuration;
using EasyLearner.Service.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace EasyLearnerAdmin.Utility.Common
{
    public class BaseController<T> : Controller where T : BaseController<T>
    {
        #region Fields
        protected JsonResponse JsonResponse => new JsonResponse();

      

        private IErrorLogService _errorLog;
        protected IErrorLogService ErrorLog => _errorLog ?? (_errorLog = HttpContext.RequestServices.GetService<IErrorLogService>());

        private IHttpContextAccessor _accessor;
        protected IHttpContextAccessor Accessor => _accessor ?? (_accessor = HttpContext.RequestServices.GetService<IHttpContextAccessor>());

        [Obsolete]
        private IHostingEnvironment _hostingEnvironment;

        [Obsolete]
        protected IHostingEnvironment HostingEnvironment => _hostingEnvironment ?? (_hostingEnvironment = HttpContext.RequestServices.GetService<IHostingEnvironment>());

        private IMapper _mapper;
        protected IMapper Mapper => _mapper ?? (_mapper = HttpContext.RequestServices.GetService<IMapper>());

        private IConfiguration _config;
        protected IConfiguration Config => _config ?? (_config = HttpContext.RequestServices.GetService<IConfiguration>());
        #endregion

        public string GetSortingColumnName(int sortColumnNo)
        {
            return Accessor.HttpContext.Request.Query["mDataProp_" + sortColumnNo][0];
        }

        public string GetPhysicalUrl()
        {
            return Config.GetValue<string>("CommonProperty:PhysicalUrl");
        }

        public string GetClientAppUrl()
        {
            return Config.GetValue<string>("CommonProperty:ClientAppUrl");
        }

        public int GetDefaultSubscriptionPlanDuration()
        {
            return Config.GetValue<int>("CommonProperty:DefaultSubscriptionDuration");
        }

        public string GetConfigValue(string key)
        {
            return Config.GetValue<string>(key);
        }

        public string GetS3ServiceUrl(string buketName, string fileName)
        {
            return $@"{Config.GetValue<string>("CommonProperty:S3ServiceUrl").Replace("{buketname}", buketName)}{fileName}";
        }
    }
    public class JsonResponse
    {
        public IActionResult GenerateJsonResult(int status, string message = null, object data = null)
        {
            return new JsonResult(new JsonResponseHelper
            {
                Status = status,
                Data = data,
                Message = message
            });
        }
    }
    public class DatatableResponseHelper
    {
        public int TotalElements { get; set; }
        public object Data { get; set; }
    }

    public class JsonResponseHelper
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
