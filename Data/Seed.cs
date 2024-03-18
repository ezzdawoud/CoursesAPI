using Courses.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace Courses.Data
{
    public static class Seed
    {
        public static void SeedUsersAndRoles(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<Users>>();
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Seed roles
                SeedRoles(roleManager);

                // Seed admin user
                SeedAdminUser(userManager);

                // Seed regular user
                SeedRegularUser(userManager);

                // Seed teacher user
                SeedTeacher(userManager);
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(UserRoles.Admin).Result)
            {
                var adminRole = new IdentityRole(UserRoles.Admin);
                roleManager.CreateAsync(adminRole).Wait();
            }

            if (!roleManager.RoleExistsAsync(UserRoles.Users).Result)
            {
                var userRole = new IdentityRole(UserRoles.Users);
                roleManager.CreateAsync(userRole).Wait();
            }

            if (!roleManager.RoleExistsAsync(UserRoles.Teacher).Result)
            {
                var teacherRole = new IdentityRole(UserRoles.Teacher);
                roleManager.CreateAsync(teacherRole).Wait();
            }
        }
        private static void SeedAdminUser(UserManager<Users> userManager)
        {
            var adminUser = userManager.FindByNameAsync("admin").Result;
            if (adminUser == null)
            {
                var user = new Users
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "Admin@123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRoles.Admin).Wait();
                }
            }
        }

        private static void SeedRegularUser(UserManager<Users> userManager)
        {
            var regularUser = userManager.FindByNameAsync("user").Result;
            if (regularUser == null)
            {
                var user = new Users
                {
                    UserName = "user",
                    Email = "user@example.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "User@123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRoles.Users).Wait();
                }
            }
        }

        private static void SeedTeacher(UserManager<Users> userManager)
        {
            var teacherUser = userManager.FindByNameAsync("teacher").Result;
            if (teacherUser == null)
            {
                var user = new Users
                {
                    UserName = "teacher",
                    Email = "teacher@example.com",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(user, "Teacher@123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRoles.Teacher).Wait();
                }
            }
        }
    }
}
