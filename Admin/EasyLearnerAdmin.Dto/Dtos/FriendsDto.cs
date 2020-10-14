using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Dto.Dtos
{
   public class FriendsDto : BaseModel
    {
        public string InvitationCode { get; set; }
        public DateTime DateOfIssue { get; set; }
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string ConsumerUserName { get; set; }
        public int NoOfFreeQuestions { get; set; }
        public int NoOfFreeDays { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
