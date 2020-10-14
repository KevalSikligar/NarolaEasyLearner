using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EasyLearner.Service.Enums
{
 public   class GlobalEnums
    {
        public enum MemberShipTypes
        {
            [Description("Annual_and_Normal")]
            Annual_and_Normal = 1,

            [Description("Annual_and_Plus")]
            Annual_and_Plus = 2,

            [Description("Monthly_and_Normal")]
            Monthly_and_Normal = 3,

            [Description("Monthly_and_Plus")]
            Monthly_and_Plus = 4
        }
    }
}
