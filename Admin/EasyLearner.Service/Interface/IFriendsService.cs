using EasyLearner.Service.Interface.BaseInterface;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;
using EasyLearner.Service.Dto;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace EasyLearner.Service.Interface
{
    public interface IFriendsService : IGenericService<Friends>
    {
        Task<List<FriendsDto>> GetFriendsList(SqlParameter[] paraObjects);

    }
}
