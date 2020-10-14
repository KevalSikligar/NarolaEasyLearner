using EasyLearnerAdmin.Data.DbModel;
using EasyLearner.Service.Interface.BaseInterface;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EasyLearner.Service.Dto;

namespace EasyLearner.Service.Interface
{
    public interface IGradeService : IGenericService<Grades>
    {
        Task<List<GradeDto>> GetGradeList(SqlParameter[] paraObjects);

    }
}
