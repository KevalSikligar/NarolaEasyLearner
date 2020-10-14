using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class QuestionRequest : EntityWithAudit
    {
        
        [Required]
        public string File { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public long StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser Students { get; set; }

        public string SubjectName { get; set; }

        public long? LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lessons Lessons { get; set; }


    }
}
