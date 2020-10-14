using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class QADto : BaseModel
    {
        public long QuestionRequestId { get; set; }
        public long QuestionResponseId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public long? GradeId { get; set; }
        public bool AnsweredQuestion { get; set; }
        public bool UnAnsweredQuestion { get; set; }

        public int ansSlectionType { get; set; }

        public long? TutorId { get; set; }


        public DateTime DateOfAns { get; set; }
        public string TimeOfAns { get; set; }
        public string LessonName { get; set; }
        public string Grade { get; set; }
        public string TutorName { get; set; }

        public string  GradeList { get; set; }

    }
}
