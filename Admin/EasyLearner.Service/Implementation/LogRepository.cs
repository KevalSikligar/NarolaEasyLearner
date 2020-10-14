using EasyLearner.Service.Dto;
using EasyLearner.Service.GlobalConstant;
using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Data.Extensions;
using EasyLearnerAdmin.Data.Utility;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearner.Service.Implementation
{
   public class LogRepository : GenericRepository<Log>, ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<StaffDto>> GetStaffList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetStaffList, paraObjects);
            return Common.ConvertDataTable<StaffDto>(dataSet.Tables[0]);
        }
        public async Task<List<StaffReportDto>> GetStaffOperationReport(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetStaffOperationReport, paraObjects);
            return Common.ConvertDataTable<StaffReportDto>(dataSet.Tables[0]);
        }
    }
}