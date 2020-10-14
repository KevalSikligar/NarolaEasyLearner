using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class DisplayGradeLessonDto:BaseModel
    {
        public string GradeName { get; set; }
        public List<string> LessonName { get; set; }
    }

    
}
