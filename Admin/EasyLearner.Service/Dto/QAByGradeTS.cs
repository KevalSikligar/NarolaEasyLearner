using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class QAByGradeTS:BaseModel
    {
        public long GradeId { get; set; }
        public string GradeName { get; set; }
        public string LessonName { get; set; }
        public long LessonId { get; set; }
        public int QACount { get; set; }
    }

    public class QAGradeStudent : BaseModel
    {
        public string GradeName { get; set; }
        public List<LessonData> Lessons{ get; set; }
       public QAGradeStudent() {
            Lessons = new List<LessonData>();
        }
    }
    public class LessonData : BaseModel
    {
        public string LessonName { get; set; }
        public int QACount { get; set; }

    }
}
