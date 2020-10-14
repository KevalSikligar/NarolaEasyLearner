using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Implementation
{
    public class TutorRelevantLessonRepository : GenericRepository<TutorRelevantLesson>, ITutorRelevantLesson
    {
        private readonly ApplicationDbContext _context;

        public TutorRelevantLessonRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
