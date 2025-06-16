using Api.Common;
using Microsoft.AspNetCore.Identity;

namespace Api.Extensions
{
    public static class RollInitializerServiceExtension
    {
        public static async Task InitializeRoleAsync(
            this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope
                .ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in SharedData.Roles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
