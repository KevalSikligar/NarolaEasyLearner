using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class StaffReportDto : BaseModel
    {
        public string staffId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StaffDate { get; set; }
    }
}
