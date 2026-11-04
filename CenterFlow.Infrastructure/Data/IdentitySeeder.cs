using CenterFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CenterFlow.Infrastructure.Data
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        public IdentitySeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task SeedAdmin()
        {
            var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            var adminExists = await userManager.FindByEmailAsync(configuration["Seed:AdminEmail"]);
            if (adminExists != null)
            {
                return;
            }
            var admin = new Admins()
            {
                UserName = "admin2",
                Email = configuration["Seed:AdminEmail"],
                EmailConfirmed =true,
                FullName ="Admin2"
            };
            var result = await userManager.CreateAsync(admin, configuration["Seed:AdminPassword"]);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));

            var roleResult = await userManager.AddToRoleAsync(admin, "Admin");
            if (!roleResult.Succeeded)
                throw new Exception(string.Join(", ", roleResult.Errors.Select(x => x.Description)));
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