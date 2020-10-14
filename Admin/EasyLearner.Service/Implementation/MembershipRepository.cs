using EasyLearner.Service.Dto;
using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearner.Service.Implementation
{
    public class MembershipRepository : GenericRepository<Membership>, IMembershipService
    {
        private readonly ApplicationDbContext _context;

        public MembershipRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

      
    }
}
