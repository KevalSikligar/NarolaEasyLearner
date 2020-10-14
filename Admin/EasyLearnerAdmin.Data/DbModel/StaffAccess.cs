using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
 public  class StaffAccess:EntityWithAudit
    {
        public long StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual Staff Staff { get; set; }

        public long MenuId { get; set; }
        [ForeignKey("MenuId")]
        public virtual MenuAccess MenuAccess { get; set; }
    }
}
