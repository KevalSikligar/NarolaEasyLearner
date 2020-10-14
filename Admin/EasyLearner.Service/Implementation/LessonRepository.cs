using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearner.Service.Implementation.BaseService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using EasyLearnerAdmin.Data.Extensions;
using EasyLearnerAdmin.Data.Utility;
using EasyLearner.Service.GlobalConstant;
using EasyLearner.Service.Dto;

namespace EasyLearner.Service.Implementation
{
    public class LessonRepository : GenericRepository<Lessons>, ILessonService
    {
        private readonly ApplicationDbContext _context;

        public LessonRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<LessonDto>> GetLessonList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetLessonList, paraObjects);
            return Common.ConvertDataTable<LessonDto>(dataSet.Tables[0]);
        }
    }
}