using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyLearner.Service.Implementation
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<List<UserDto>> GetUserList(SqlParameter[] paraObjects)
        //{
        //    var dataSet = await _context.GetQueryDatatableAsync(StoredProcedureList.GetUserList, paraObjects);
        //    return Common.ConvertDataTable<UserDto>(dataSet.Tables[0]);
        //}
    }
}
