using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
  public  class SupportResponseDto:BaseModel
    {
        public string SubjectOfRequest { get; set; }
        public DateTime RequestDateTime { get; set; }
        public string RequestDescription { get; set; }

        public string SubjectOfResponse { get; set; }

        public DateTime ResponseDateTime { get; set; }
        public string ResponseDescription { get; set; }
        public long StaffId { get; set; }
        public long RequestId { get; set; }
       
    }
}
