using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
   public class GradeDto:BaseModel
    {
        public string Name { get; set; }
        public List<LessonDto> lessonList { get; set; }
    }
}
