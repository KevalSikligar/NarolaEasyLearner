using System;
using System.Collections.Generic;
using System.Text;
using static EasyLearner.Service.Enums.GlobalEnums;

namespace EasyLearner.Service.Dto
{
    public class FinancialDto : BaseModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal price { get; set; }
        public int MemberShipTypes { get; set; }
        public decimal totalAmount { get; set; }
        public int totalPerson { get; set; }

        public List<FinancialDto> ViewReportList { get; set; }


    }
    
}

