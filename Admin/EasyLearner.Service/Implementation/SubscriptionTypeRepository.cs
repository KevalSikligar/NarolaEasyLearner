using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Implementation
{
    public class SubscriptionTypeRepository : GenericRepository<SubscriptionType>, ISubscriptionTypeService
    {

        private readonly ApplicationDbContext _context;

        public SubscriptionTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
