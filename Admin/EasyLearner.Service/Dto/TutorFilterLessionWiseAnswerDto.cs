using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class TutorFilterLessionWiseAnswerDto : BaseModel
    {
        public long Tid { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public  string lessonName { get; set; }

        public int numberOfAnswer { get; set; }
    }
}
