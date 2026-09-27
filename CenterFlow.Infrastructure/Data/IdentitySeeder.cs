using Microsoft.AspNetCore.Identity;

namespace CenterFlow.Infrastructure.Data
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> roleManager;
        public IdentitySeeder(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public async Task SeedRole()
        {
            string[] roles = { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                var exists = await roleManager.RoleExistsAsync(role);
                if (!exists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}