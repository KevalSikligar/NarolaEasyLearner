using EasyLearnerAdmin.Data.DbModel;
using EasyLearner.Service.Interface.BaseInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using EasyLearner.Service.Dto;

namespace EasyLearner.Service.Interface
{
    public interface ILessonService : IGenericService<Lessons>
    {
        Task<List<LessonDto>> GetLessonList(SqlParameter[] paraObjects);
    }
}
