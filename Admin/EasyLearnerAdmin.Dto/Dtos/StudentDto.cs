using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Dto.Dtos
{
    public class StudentDto : BaseModel
    {
        public string StudentName { get; set; }
        public string Subscription { get; set; }

        public string GradeName { get; set; }
        public long GradeId { get; set; }

        public bool MemberShipStatus { get; set; }
        public string StudentStatus { get; set; }
        public int PaymentAmount { get; set; }
        public string PaymentExplanation { get; set; }
    }
   
}
