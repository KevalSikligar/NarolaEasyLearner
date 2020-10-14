using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class Grades:EntityWithAudit
    {
    
          [Required,StringLength(100)]  
          public string Name { get; set; }
   
          
    }
}
