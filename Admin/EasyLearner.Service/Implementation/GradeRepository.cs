using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyLearnerAdmin.Data.Utility;
using EasyLearnerAdmin.Data.Extensions;
using EasyLearner.Service.Dto;
using EasyLearner.Service.GlobalConstant;

namespace EasyLearner.Service.Implementation
{
  
    public class GradeRepository : GenericRepository<Grades>, IGradeService
    {
        private readonly ApplicationDbContext _context;

        public GradeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<GradeDto>> GetGradeList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetGradeList, paraObjects);
            return Common.ConvertDataTable<GradeDto>(dataSet.Tables[0]);
        }

    }
}
