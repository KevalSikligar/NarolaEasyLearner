using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class QuestionResponse : EntityWithAudit
    {
        
        public long? TutorId { get; set; }
        [ForeignKey("TutorId")]
        public virtual ApplicationUser Tutors{get;set;}

        [Required]
        public long QuestionRequestId { get; set; }
        [ForeignKey("QuestionRequestId")]
        
        public virtual QuestionRequest QuestionRequest{ get; set; }
        public string Response { get; set; }
        public string File { get; set; }
    }
}
