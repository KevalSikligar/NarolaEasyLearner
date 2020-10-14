using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EasyLearner.Service.Dto
{
    public class MessageDto:BaseModel
    {
        public string MessageText { get; set; }
        public string StudentIdList { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long UserId { get; set; }
    }
}
