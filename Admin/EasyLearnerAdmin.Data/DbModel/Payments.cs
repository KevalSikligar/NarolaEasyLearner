using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
   public class Payments : EntityWithAudit
    {
        public long? TutorId { get; set; }
        [ForeignKey("TutorId")]
        public virtual Tutors Tutors { get; set; }

        public long? StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual Staff Staff { get; set; }
        [StringLength(500)]

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public string Description { get; set; }
    }
}
