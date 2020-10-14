using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class Membership: EntityWithAudit
    {
        [Required]
        public long MemberShipTypeId { get; set; }
        [ForeignKey("MemberShipTypeId")]
        public virtual SubscriptionType SubscriptionType { get; set; }

        public long StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Students Students { get; set; }    

        [Required]
        public DateTime ExprireDate { get; set; }

        public bool MembershipStatus { get; set; }

    }
}
