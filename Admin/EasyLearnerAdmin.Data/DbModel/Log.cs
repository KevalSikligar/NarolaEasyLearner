using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
 public  class Log:EntityWithAudit
    {
        public long StaffId { get; set; }
        [ForeignKey("StaffId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        public string Description { get; set; }

    }
}
