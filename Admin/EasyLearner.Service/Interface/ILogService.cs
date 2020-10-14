using EasyLearner.Service.Dto;
using EasyLearner.Service.Interface.BaseInterface;
using EasyLearnerAdmin.Data.DbModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearner.Service.Interface
{
    public interface ILogService:IGenericService<Log>
    {
        Task<List<StaffDto>> GetStaffList(SqlParameter[] paraObjects);
        Task<List<StaffReportDto>> GetStaffOperationReport(SqlParameter[] paraObjects);
    }
}
