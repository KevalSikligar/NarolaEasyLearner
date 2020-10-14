using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class TutorDto : BaseModel
    {
        public string LessonId { get; set; }
        public string LessonName { get; set; }
        public long UserId { get; set; }
        //public long GradeId { get; set; }
        public string RelevantLesson { get; set; }
        public string TutorName { get; set; }
        public string MobileNo { get; set; }
        public string UserName { get; set; }
        public string TutorPassword { get; set; }
        public DateTime DateofEmployeement { get; set; }
        public string employmentDate { get; set; }
        public long? RowNumber { get; set; }
        public int? NoOfAnswerCount { get; set; }

    }
    public class RelevantLessonDto
    {
        public long LessonId { get; set; }
        public long TutorId { get; set; }
    }

}
