using EasyLearner.Service.Implementation.BaseService;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbModel;
using System;
using System.Collections.Generic;
using System.Text;


namespace EasyLearner.Service.Implementation
{
   public class SettingRepository : GenericRepository<Settings>, ISettingService
    {
        private readonly ApplicationDbContext _context;

        public SettingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void AddSettings(Settings objsettings)
        {
            throw new NotImplementedException();
        }
    }
}
