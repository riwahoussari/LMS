using LMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace LMS.Infrastructure.Seeding
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeeder(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public static string[] Roles = { "admin", "tutor", "student" };
        public async Task SeedRolesAsync()
        {
            foreach (var roleName in Roles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                }
            }
        }
    }
}
