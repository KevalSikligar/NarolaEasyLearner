using EasyLearner.Service.Dto;
using EasyLearner.Service.GlobalConstant;
using EasyLearner.Service.Interface;
using EasyLearnerAdmin.Data.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyLearnerAdmin
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, Role>
    {
        private readonly IStaffService _staff;
        private readonly IStaffAccessService _staffAccess;
        private readonly IMenuAccessService _menu;

        public ClaimsPrincipalFactory(IStaffService staffService, IStaffAccessService staffAccessService, IMenuAccessService menuAccessService, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
            _staff = staffService;
            _staffAccess = staffAccessService;
            _menu = menuAccessService;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {


            var identity = await base.GenerateClaimsAsync(user);
            var role = identity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList().FirstOrDefault()?.Value ?? "";
            //string AdminId = (role == UserRoles.SystemAdmin) ? user.Id.ToString(): "";
            //string CompanyId = (role == UserRoles.Company) ? user.Id.ToString(): "";
            //string EmployeeId = (role == UserRoles.Employee) ? user.Id.ToString(): "";
            var claims = new List<Claim>()
            {
                new Claim("UserRole", role),
                new Claim("UserId", user.Id.ToString() ?? ""),
                new Claim("FullName", user.FullName ??"" )

            };
            identity.AddClaims(claims);
            if (user != null)
            {

                var staff = await _staff.GetSingleAsync(x => x.UserId == user.Id);
                if (staff != null)
                {
                    var staffAccessList = _staffAccess.GetAll(x => x.StaffId == staff.Id).Select(x => x.MenuId);
                    var menuList = _menu.GetAll(x => staffAccessList.Contains(x.Id)).Select(x => x.MenuName);
                    StaffDto staffAccess = new StaffDto();
                    foreach (var menu in menuList)
                    {
                        switch (menu)
                        {
                            case MenuAccessConstant.Student: staffAccess.Student = true; break;
                            case MenuAccessConstant.Exams: staffAccess.Exams = true; break;
                            case MenuAccessConstant.Financial: staffAccess.Financial = true; break;
                            case MenuAccessConstant.OutBox: staffAccess.OutBox = true; break;
                            case MenuAccessConstant.QA: staffAccess.QA = true; break;
                            case MenuAccessConstant.Setting: staffAccess.Setting = true; break;
                            case MenuAccessConstant.Support: staffAccess.Support = true; break;
                            case MenuAccessConstant.Tutor: staffAccess.Tutor = true; break;
                            default: break;

                        }

                    }

                    var claimsAccess = new List<Claim>()
            {

                new Claim("StudentMenu", staffAccess.Student?"GRANTED":"DENIED" ),
                new Claim("ExamsMenu", staffAccess.Exams?"GRANTED":"DENIED" ),
                new Claim("FinancialMenu", staffAccess.Financial?"GRANTED":"DENIED" ),
                new Claim("OutBoxMenu", staffAccess.OutBox?"GRANTED":"DENIED" ),
                new Claim("QAMenu", staffAccess.QA?"GRANTED":"DENIED" ),
                new Claim("SupportMenu", staffAccess.Support?"GRANTED":"DENIED" ),
                new Claim("TutorMenu", staffAccess.Tutor?"GRANTED":"DENIED" ),
                new Claim("SettingMenu", staffAccess.Setting?"GRANTED":"DENIED" ),
            };
                    identity.AddClaims(claimsAccess);
                }
            }
            return identity;
        }
    }
}
