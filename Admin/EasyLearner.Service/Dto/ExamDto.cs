using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
 public   class ExamDto:BaseModel
    {
        public string ExamNameAndYear { get; set; }
        public string ExamYear { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string FileName { get; set; }
        public IFormFile ExamFile { get; set; }
        public long ExamId { get; set; }
        public long QuestionId { get; set; }
        public long ResponseId { get; set; }
        public string LessonIdList { get; set; }

        public string QuestionDescription { get; set; }
        public string ResponseDescription { get; set; }

        public string QuestionFileName { get; set; }
        public string ResponseFileName { get; set; }
        public string SubjectName { get; set; }
    }
}
