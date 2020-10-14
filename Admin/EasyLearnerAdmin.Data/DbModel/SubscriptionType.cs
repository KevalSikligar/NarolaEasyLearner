using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class SubscriptionType: EntityWithAudit
    {
        [Required]
        public int   TypeId { get; set; }

        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public int AllowedQuestion { get; set; }
        [Required]
        public int AllowedDays { get; set; }

    }
}
