using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Dto.Dtos
{
    public class TutorDto:BaseModel
    {
        public long GradeId { get; set; }
        public string GradeName { get; set; }

        public string TutorName { get; set; }

        public string UserName { get; set; }
    }
}
