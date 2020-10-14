using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
   public class StudentFilterDto : BaseModel
    {

        public bool IsMember { get; set; }
        public bool IsNotMember { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
