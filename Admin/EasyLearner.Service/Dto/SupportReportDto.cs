using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class SupportReportDto:BaseModel
    {
        public DateTime DateOfSend { get; set; }
        public string TimeOfSend { get; set; }
        public string StudentUserName { get; set; }
        public string SubjectOfSupport { get; set; }
        public long RequestId { get; set; }

        public string StudentName { get; set; }
        public string Description { get; set; }
    }
}
