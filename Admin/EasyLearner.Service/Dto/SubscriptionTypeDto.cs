using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class SubscriptionTypeDto : BaseModel
    {
        public int TypeId { get; set; }
        public decimal Price { get; set; }
        public int AllowedQuestion { get; set; }
        public int AllowedDays { get; set; }
        public long Resultcount { get; set; }
    }
}
