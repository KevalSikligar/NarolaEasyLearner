using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class InvitationCode: EntityWithAudit
    {
        [Required]
        public int NumberOfFreeQuestions { get; set; }

        [Required]
        public int NumberOfFreeDays { get; set; }

        [Required]
        public int ExpirationDays { get; set; }
    }
}
