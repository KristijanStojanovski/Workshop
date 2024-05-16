using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Workshop.Areas.Identity.Data;
using Workshop.Data;

namespace Workshop.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<WorkshopUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            WorkshopUser admin = await UserManager.FindByEmailAsync("admin@example.com");
            if (admin == null)
            {
                var User = new WorkshopUser();
                User.Email = "admin@example.com";
                User.UserName = "admin@example.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WorkshopContext(
                serviceProvider.GetRequiredService<DbContextOptions<WorkshopContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
            }
        }
    }
}
