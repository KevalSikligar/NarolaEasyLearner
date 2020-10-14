using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class SubscriptionDto:BaseModel
    {
        //public long StudentId { get; set; }
        //public string StudentName { get; set; }

        public long SubscriptionTypeId { get; set; }
        //public string SubscriptionName { get; set; }
        //public DateTime ExpiryDate { get; set; }

        public string MembershipType1 { get; set; }
        public string MembershipType2 { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int AllowedQuestion { get; set; }
    }
}
