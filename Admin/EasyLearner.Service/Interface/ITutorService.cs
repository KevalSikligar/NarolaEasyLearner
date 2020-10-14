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
    public interface ITutorService : IGenericService<Tutors>
    {
        Task<List<TutorDto>> GetTutorList(SqlParameter[] paraObjects);

        Task<List<TutorDto>> GetFilterTutorReport(SqlParameter[] paraObjects);
        Task<List<TutorFilterLessionWiseAnswerDto>> GetCountWiseTutorReport(SqlParameter[] paraObjects);
    }
}
