using EasyLearner.Service.Dto;
using EasyLearner.Service.Interface.BaseInterface;
using EasyLearnerAdmin.Data.DbModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearner.Service.Interface
{
    public interface IStudentService : IGenericService<Students>
    {
        Task<List<StudentDto>> GetStudentList(SqlParameter[] paraObjects);

        Task<List<StudentDto>> GetFilterStudentList(SqlParameter[] paraObjects);

        Task<List<StudentFilterGradeWiseDto>> GetGradeWiseStudentFilter(SqlParameter[] paraObjects);
        Task<List<QAByGradeTS>> GetQAByGrade(SqlParameter[] paraObjects);

        Task<List<FriendsDto>> GetStudentInviteFriendList(SqlParameter[] paraObjects);

    }
}
