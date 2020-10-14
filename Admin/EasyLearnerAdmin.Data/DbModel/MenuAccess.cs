using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
 public  class MenuAccess:EntityWithAudit
    {
        public string MenuName { get; set; }
        
        public virtual ICollection<StaffAccess> StaffAccesses { get; set; }   
        

    }
}
