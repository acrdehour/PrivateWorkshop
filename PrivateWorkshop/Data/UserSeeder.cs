using Microsoft.AspNetCore.Identity;
using PrivateWorkshop.Constants;

namespace PrivateWorkshop.Data
{
    public class UserSeeder
    {
        public static async Task SeedUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await CreateUserWithRole(userManager, "admin@privateworkshop.com", "!456Pass", Roles.Admin);
            await CreateUserWithRole(userManager, "clientno1@privateworkshop.com", "!456Pass", Roles.Client);
        }
        public static async Task CreateUserWithRole(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception($"Failes to creating user with email {user.Email}. Errors {string.Join(",", result.Errors)}");
                }
            }

        }
    }
}
