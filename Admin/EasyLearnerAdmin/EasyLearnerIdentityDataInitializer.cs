using EasyLearnerAdmin.Data.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLearnerAdmin
{
    public class EasyLearnerIdentityDataInitializer
    {
   
        public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var user = new ApplicationUser
            {
                FirstName = "Aman",
                LastName = "Sharma",
                MiddleName = "",
                UserName = "ams@narola.email",
                MobileNumber = "+1 999-999-9999",
                NormalizedUserName = "Admin",
                Email = "ams@narola.email",
                NormalizedEmail = "ams@narola.email",
                EmailConfirmed = true,
                IsActive = true,
                AddressLine1 = "Surat",

            };
            if (userManager.FindByEmailAsync(user.UserName).Result != null) return;

            var result = userManager.CreateAsync(user, "Password123#").Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(user, "Admin").Wait();
            }



            var EndUser = new ApplicationUser
            {
                FirstName = "Swagatam",
                LastName = "Narola",
                MiddleName = "",
                UserName = "sbe@narola.email",
                MobileNumber = "+1 999-999-9999",
                NormalizedUserName = "user",
                Email = "sbe@narola.email",
                NormalizedEmail = "sbe@narola.email",
                EmailConfirmed = true,
                IsActive = true,
                AddressLine1 = "Surat",
            };
            if (userManager.FindByEmailAsync(EndUser.UserName).Result != null) return;
            var developeResult = userManager.CreateAsync(EndUser, "Password123#").Result;

            if (developeResult.Succeeded)
            {
                userManager.AddToRoleAsync(EndUser, "Admin").Wait();
            }


        }

        private static void SeedRoles(RoleManager<Role> roleManager)
        {
            #region User Roles
            Dictionary<string, string> normalizedName = new Dictionary<string, string>
            {
                { "Admin", "Admin"},
                { "Tutor", "Tutor"},
                { "Student", "Student"},
                { "Staff", "Staff"},
            };

            var existrolesList = roleManager.Roles.Select(x => x.Name).ToList();
            if (existrolesList.Any())
            {
                var notExirst = normalizedName.Keys.Except(existrolesList);
                foreach (var notRole in notExirst)
                {
                    string normalized = normalizedName.FirstOrDefault(x => x.Key == notRole).Value;
                    var roleResult = roleManager.CreateAsync(new Role { Name = notRole, NormalizedName = normalized, DisplayRoleName = normalized }).Result;
                }
            }
            else
            {
                foreach (var objRole in normalizedName.Keys)
                {
                    string normalized = normalizedName.FirstOrDefault(x => x.Key == objRole).Value;
                    IdentityResult roleResult = roleManager.CreateAsync(new Role { Name = objRole, NormalizedName = normalized, DisplayRoleName = normalized }).Result;
                }
            }
            #endregion
        }

        public static long GetTimeZoneId(string currentTimeZone, string connection)
        {
            using (var sqlCon = new SqlConnection(connection))
            {
                var query = $@"EXEC GetCurrentTimeZone '{currentTimeZone}';";
                sqlCon.Open();
                var sqlCmd = new SqlCommand(query, sqlCon);
                var result = sqlCmd.ExecuteScalar();
                sqlCon.Close();
                return (long?)result ?? 0;
            }
        }
    }
}
