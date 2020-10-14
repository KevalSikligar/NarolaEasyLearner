using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace EasyLearner.Service.Dto
{
   public class TutorFiltorDto : BaseModel
    {
        public bool TutorisActive { get; set;}
        public bool TutorisInActive { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
