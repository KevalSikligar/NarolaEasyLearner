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
   public class ExamRepository : GenericRepository<Exams>, IExamService
    {
        private readonly ApplicationDbContext _context;

        public ExamRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<ExamDto>> GetExamList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetExamList, paraObjects);
            return Common.ConvertDataTable<ExamDto>(dataSet.Tables[0]);
        }
        public async Task<List<ExamDto>> GetQuestionList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetQuestionList, paraObjects);
            return Common.ConvertDataTable<ExamDto>(dataSet.Tables[0]);
        }

    }
}
