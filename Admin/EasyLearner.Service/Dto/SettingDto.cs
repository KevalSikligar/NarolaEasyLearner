using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class SettingDto:BaseModel
    {
        public string Title { get; set;}

        public string Html { get; set; }

        public long UserId { get; set; }
    }
}
