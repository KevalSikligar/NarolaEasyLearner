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
   public class QuestionResponseRepository : GenericRepository<QuestionResponse>, IQuestionResponseService
    {
        private readonly ApplicationDbContext _context;

        public QuestionResponseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<QADto>> GetQAReport(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetQADateWiseReport, paraObjects);
            return Common.ConvertDataTable<QADto>(dataSet.Tables[0]);
        }
    }
}

   
   