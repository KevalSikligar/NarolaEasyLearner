using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class StudentDto : BaseModel
    {

        public string StudentName { get; set; }
        public int? Subscription { get; set; }

        public string  UserName { get; set; }
        public string UserPassword { get; set; }

        public string GradeName { get; set; }
        [Required]
        public long GradeId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public long UserId { get; set; }
        public bool MembershipStatus { get; set; }
        public string StudentStatus { get; set; }
        public int? PaymentAmount { get; set; }
        public string PaymentExplanation { get; set; }
        public int? NoOfDays { get; set; }
        public int? NoOfQuestions { get; set; }
        public long? RowNumber { get; set; }
        public DateTime? ExprireDate { get; set; }

    }

    public class FilterTutor
    {
        [Required]
        public bool MembershipStatus { get; set; }
        [Required]
        public string FromDate { get; set; }
        [Required]
        public string ToDate { get; set; }
    }

}
