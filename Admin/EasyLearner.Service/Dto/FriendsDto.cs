using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EasyLearner.Service.Dto
{
   public class FriendsDto:BaseModel
    {
        public string InvitationCode { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public long? StudentId { get; set; }
        //public string StudentName { get; set; }
        public string ConsumerUserName { get; set; }
       
    }
}
