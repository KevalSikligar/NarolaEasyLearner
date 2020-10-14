using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class PaymentDto:BaseModel
    {
        public long? TutorId { get; set; }
        public long? StaffId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }
        public string TutorName { get; set; }
        public string StaffName { get; set; }
        public string UserName { get; set; }

    }
}
