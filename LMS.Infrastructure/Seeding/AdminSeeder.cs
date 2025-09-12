using LMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Seeding
{
    public class AdminSeeder
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminSeeder(IConfiguration config, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        
        public async Task SeedAdminAsync()
        {
           
            // Bind Admin config
            var adminConfig = _config.GetSection("SuperAdmin");

          
            // Check if admin already exists
            var adminUser = await _userManager.FindByNameAsync(adminConfig["Username"]!);
            if (adminUser == null)
            {
                var adminRole = await _roleManager.FindByNameAsync("admin");
                var newAdmin = new AppUser
                {
                    UserName = adminConfig["Username"],
                    Email = adminConfig["Email"],
                    FirstName = adminConfig["FirstName"]!,
                    LastName = adminConfig["LastName"]!,
                    EmailConfirmed = true,
                    RoleId = adminRole.Id
                };

                var result = await _userManager.CreateAsync(newAdmin, adminConfig["Password"]!);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newAdmin, "admin");
                }
                else
                {
                    throw new Exception("Failed to create admin user: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

    }
}
