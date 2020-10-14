using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Dto.Enums
{
    public class StoredProcedureList
    {
        public const string GetGradeList = @"GetGradeList";
        public const string GetLessonList = @"GetLessonList";


    }
    public class LoginValidationMessageList
    {
        public const string WrongUserNamePassword= "Your Username and password is not correct please try again";
        public const string InactivateUser = "Your Username and Password is not Active! please call the Company";
    }
}
