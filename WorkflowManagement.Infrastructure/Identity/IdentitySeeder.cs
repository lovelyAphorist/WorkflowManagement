using Microsoft.AspNetCore.Identity;
using WorkflowManagement.Application.Users.Constants;

namespace WorkflowManagement.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            var roles = new[]
            {
                AppRoles.Admin,
                AppRoles.Member
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole<Guid>
                        {
                            Id = Guid.NewGuid(),
                            Name = role
                        });
                }
            }

            var admin = await userManager.FindByEmailAsync(
                "megan@megan.com");

            if (admin is not null &&
                !await userManager.IsInRoleAsync(admin, AppRoles.Admin))
            {
                await userManager.AddToRoleAsync(
                    admin,
                    AppRoles.Admin);
            }

            var member = await userManager.FindByEmailAsync(
                "megan@example.com");

            if (member is not null &&
                !await userManager.IsInRoleAsync(member, AppRoles.Member))
            {
                await userManager.AddToRoleAsync(
                    member,
                    AppRoles.Member);
            }
        }
    }
}