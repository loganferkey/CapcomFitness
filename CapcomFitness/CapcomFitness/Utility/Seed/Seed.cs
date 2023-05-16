using CapcomFitness.Data;
using CapcomFitness.Models;
using Microsoft.AspNetCore.Identity;

namespace CapcomFitness.Utility.Seed
{
    public class Seed
    {
        public static async Task SeedRoles(IApplicationBuilder appBuilder)
        {
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                // Using role manager from the created service scope, requires app builder OR role manager (maybe dependency injection for website use?)
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // If they don't exist already create new admin and user roles
                if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
        }

        public static async Task SeedUsers(IApplicationBuilder appBuilder)
        {
            using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
            {
                // Using user manager from service scope, requires app builder OR user manager!
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                string email = "admin@flex.com";
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var newAdminUser = new AppUser()
                    {
                        UserName = "admin",
                        Email = email,
                        EmailConfirmed = true,
                        Age = 21,
                        Nickname = "admin",
                        Joined = DateTime.Now
                    };
                    await userManager.CreateAsync(newAdminUser, "admin");
                    await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
                }

                email = "user@flex.com";
                user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    var newUser = new AppUser()
                    {
                        UserName = "user",
                        Email = email,
                        EmailConfirmed = true,
                        Age = 20,
                        Nickname = "user",
                        Joined = DateTime.Now
                    };
                    await userManager.CreateAsync(newUser, "user");
                    await userManager.AddToRoleAsync(newUser, UserRoles.User);
                }
            }
        }
    }
}
