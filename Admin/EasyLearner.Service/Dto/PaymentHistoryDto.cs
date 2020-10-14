using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
   public class PaymentHistoryDto:BaseModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public bool AllTutor { get; set; }
        public bool AllStaff { get; set; }
        public long StaffId { get; set; }

        public long TutorId { get; set; }
        public decimal price { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }
        public string TutorName { get; set; }
        public string StaffName { get; set; }

        public decimal totalAmount { get; set; }
        public decimal Amount { get; set; }
        public List<PaymentHistoryDto> ViewReportList { get; set; }
    }
}
