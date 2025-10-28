using Microsoft.AspNetCore.Identity;
using PrivateWorkshop.Constants;
using System.Data;

namespace PrivateWorkshop.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Client))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Client));
            }
        }
    }
}
