using EasyLearner.Service.Dto;
using EasyLearner.Service.GlobalConstant;
using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using EasyLearnerAdmin.Data.Extensions;
using EasyLearnerAdmin.Data.Utility;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearner.Service.Implementation
{
   public class FriendsRepository : GenericRepository<Friends>, IFriendsService
    {
        private readonly ApplicationDbContext _context;

        public FriendsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<FriendsDto>> GetFriendsList(SqlParameter[] paraObjects)
        {
            var dataSet = await _context.GetQueryDatatableAsync(SpConstants.GetFriendsList, paraObjects);
            return Common.ConvertDataTable<FriendsDto>(dataSet.Tables[0]);
        }
    }
}
