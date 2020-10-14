using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
   public class ExamLessons:EntityWithAudit
    {
        [Required]
        public long ExamQuestionId { get; set; }
        [ForeignKey("ExamQuestionId")]
        public virtual ExamsQuestions ExamsQuestions { get; set; }

        [Required]
        public long LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lessons Lessons { get; set; }

    }
}
