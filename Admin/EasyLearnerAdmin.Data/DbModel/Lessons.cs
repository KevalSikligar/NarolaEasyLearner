using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
   public class Lessons: EntityWithAudit
    {
        [Required,StringLength(250)]
        public string Name { get; set; }

        public string FileName { get; set; }

        [Required]
        public long GradeId { get; set; }
        [ForeignKey("GradeId")]
        public virtual Grades Grades { get; set; }
    }
}
