using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class UserDto : BaseModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int TutorId { get; set; }
        public string TutorName { get; set; }
    }
}
