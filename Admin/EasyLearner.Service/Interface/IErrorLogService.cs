using EasyLearnerAdmin.Data.DbModel;
using EasyLearner.Service.Interface.BaseInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Interface
{
    public interface IErrorLogService : IGenericService<ErrorLog>
    {
        void AddErrorLog(System.Exception ex, string appType);
    }
}
