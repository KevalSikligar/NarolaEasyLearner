using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Dto
{
   public class StudentFilterGradeWiseDto : BaseModel
    {
       
        public string GradeName { get; set;}

        public long NumberOfStudent { get; set; }
    }
}
