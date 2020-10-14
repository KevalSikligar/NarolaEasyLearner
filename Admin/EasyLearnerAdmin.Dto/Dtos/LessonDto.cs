using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Dto.Dtos
{
    public class LessonDto : BaseModel
    {
        public string Name { get; set; }
        public string GradeName { get; set; }
        public long GradeId { get; set; }
    }
}
