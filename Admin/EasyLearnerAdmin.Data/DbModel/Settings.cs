using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class Settings:EntityWithAudit
    {
        [Required,StringLength(250)]
        public string Title { get; set; }
        [Required]
        public string Html { get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
    }
}
