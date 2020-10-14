using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Implementation
{
   public class InvitationCodeRepository : GenericRepository<InvitationCode>, IInvitationCodeService
    {
        private readonly ApplicationDbContext _context;
        public InvitationCodeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
