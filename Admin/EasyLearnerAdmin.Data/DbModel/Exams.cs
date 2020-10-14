using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
   public class Exams: EntityWithAudit
    {
        [Required,StringLength(50)]
        public string ExamNameAndYear { get; set; }
      
        
    }
}
