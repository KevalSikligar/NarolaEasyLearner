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
  public  class QuestionRequestRepository : GenericRepository<QuestionRequest>, IQuestionRequestService
    {
        private readonly ApplicationDbContext _context;

        public QuestionRequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
       
    }
}

   