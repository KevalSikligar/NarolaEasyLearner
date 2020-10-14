using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class Students : EntityWithAudit
    {
      
       
        public string StudentName { get; set; }
        public string StudentStatus { get; set; }
        public int? Subscription { get; set; }
        public int? PaymentAmount { get; set; }
        public string PaymentExplanation { get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        
        public long GradeId { get; set; }
        [ForeignKey("GradeId")]
        public virtual Grades Grade { get; set; }

        public virtual ICollection<Membership> Memberships { get; set; }
    }
}
