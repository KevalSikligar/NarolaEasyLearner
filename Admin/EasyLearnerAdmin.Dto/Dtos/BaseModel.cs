using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Dto.Dtos
{
    public class BaseModel
    {
        public long Id { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string DateCreated { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public long TotalRecords { get; set; }
    }
}
