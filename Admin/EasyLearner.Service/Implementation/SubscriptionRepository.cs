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
   public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionService
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<SubscriptionDto>> GetSubscriptionList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetSubscriptionList, paraObjects);
            return Common.ConvertDataTable<SubscriptionDto>(dataSet.Tables[0]);
        }
    }
}
