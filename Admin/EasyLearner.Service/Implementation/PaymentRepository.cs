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
   public class PaymentRepository : GenericRepository<Payments>, IPaymentService
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<PaymentHistoryDto>> GetPaymentByTutorList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetPaymentHistoryByTutor, paraObjects);
            return Common.ConvertDataTable<PaymentHistoryDto>(dataSet.Tables[0]);
        }
        public async Task<List<PaymentHistoryDto>> GetPaymentByStaffList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetPaymentHistoryByStaff, paraObjects);
            return Common.ConvertDataTable<PaymentHistoryDto>(dataSet.Tables[0]);
        }

    }
}
