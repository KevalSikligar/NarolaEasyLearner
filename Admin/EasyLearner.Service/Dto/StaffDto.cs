using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class StaffDto : BaseModel
    {
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string Mobile { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool Setting { get; set; }
        public bool Student { get; set; }
        public bool Tutor { get; set; }
        public bool QA { get; set; }
        public bool Support { get; set; }
        public bool OutBox { get; set; }
        public bool Financial { get; set; }
        public bool Exams { get; set; }
        public bool isEdit { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public long UserId { get; set; }

        public string DateOfEmployment { get; set; }


    }
}
