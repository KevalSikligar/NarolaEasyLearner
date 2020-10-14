using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class AttemptedExams: EntityWithAudit
    {
        public long StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }

        public long ExamId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exams Exams { get; set; }
        [Required]
        public decimal Score { get; set; }
    }
}
