using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
   public class SupportResponse: EntityWithAudit
    {
        public long RequestId { get; set; }
        [ForeignKey("RequestId")]
        public virtual SupportRequest SupportRequest { get; set; }
        [Required]
        public string Description { get; set; }
        public string File { get; set; }

        public string Subject { get; set; }

        public long? StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }

        [Required]
        public TimeSpan ResponseTime { get; set; }
    }
}
