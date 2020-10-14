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
    public class SupportRequestRepository : GenericRepository<SupportRequest>, ISupportRequestService
    {
        private readonly ApplicationDbContext _context;

        public SupportRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SupportReportDto>> GetSupportReportList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetSupportReportList, paraObjects);
            return Common.ConvertDataTable<SupportReportDto>(dataSet.Tables[0]);
        }
        public async Task<List<SupportResponseDto>> GetSupportHistoryList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetSupportHistoryList, paraObjects);
            return Common.ConvertDataTable<SupportResponseDto>(dataSet.Tables[0]);
        }

    }
}