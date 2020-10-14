using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Dto.Dtos
{
    public class UserDto:BaseModel
    {
        public int  StudentId { get; set; }
        public string StudentName { get; set; }
        public int TutorId { get; set; }
        public string TutorName { get; set; }
    }
}
