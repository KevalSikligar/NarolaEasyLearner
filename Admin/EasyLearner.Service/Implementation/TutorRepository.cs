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
    public class TutorRepository: GenericRepository<Tutors>, ITutorService
    {
        private readonly ApplicationDbContext _context;

        public TutorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<TutorDto>> GetFilterTutorReport(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetTutorFilterReport, paraObjects);
            return Common.ConvertDataTable<TutorDto>(dataSet.Tables[0]);
        }
        public async Task<List<TutorDto>> GetTutorList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetTutorList, paraObjects);
            return Common.ConvertDataTable<TutorDto>(dataSet.Tables[0]);
        }
        public async Task<List<TutorFilterLessionWiseAnswerDto>> GetCountWiseTutorReport(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetCountWiseTutorReport, paraObjects);
            return Common.ConvertDataTable<TutorFilterLessionWiseAnswerDto>(dataSet.Tables[0]);
        }
    }
}
