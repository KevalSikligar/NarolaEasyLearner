using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class SupportRequest: EntityWithAudit
    {
        public long StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        [Required,StringLength(250)]
        public string Subject { get; set; }
        [Required]
        public string Description { get; set; }
        public string File { get; set; }
    }
}
