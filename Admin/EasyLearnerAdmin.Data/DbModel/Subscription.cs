using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class Subscription : EntityWithAudit
    {
        public long SubscriptionTypeId { get; set; }
        [ForeignKey("SubscriptionTypeId")]
        public virtual SubscriptionType SubscriptionType { get; set; }

        public long StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
    }
}
