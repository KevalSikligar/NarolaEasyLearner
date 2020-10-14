using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class ExamsQuestions : EntityWithAudit
    {
        
        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer{ get; set; }

        public string FileName { get; set; }

        [Required]
        public long ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exams Exams { get; set; }

    }
}
