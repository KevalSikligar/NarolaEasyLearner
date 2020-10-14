using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class Friends : EntityWithAudit
    {
        
        public string InvitationCode { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public DateTime? DateOfUse { get; set; }

        public long? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; }

        public string ConsumerUserName { get; set; }
      
    }
}
