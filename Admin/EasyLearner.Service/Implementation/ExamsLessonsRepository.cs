using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Implementation
{
    public class ExamsLessonsRepository : GenericRepository<ExamLessons>, IExamLessonService
    {
        private readonly ApplicationDbContext _context;

        public ExamsLessonsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    
    }
}
