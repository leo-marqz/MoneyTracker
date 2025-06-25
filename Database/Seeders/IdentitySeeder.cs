
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MoneyTracker.Models;

namespace MoneyTracker.Database.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<Role>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            string[] roles = { SystemRole.ADMIN, SystemRole.USER };

            // Crea los roles si no existen
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var identityRole = new Role { Name = role };
                    await roleManager.CreateAsync(identityRole);
                }
            }

            // Crea un usuario administrador si no existe
            var password = "$ABClm123$";

            var userAdmin = new User
            {
                Firstname = "Leonel",
                Lastname = "Marquez",
                Email = "leomarqz2020@gmail.com",
                UserName = "leomarqz2020@gmail.com",
                EmailConfirmed = true
            };

            var existingUser = await userManager.FindByEmailAsync(userAdmin.Email);

            if (existingUser == null)
            {
                // Crea el usuario administrador
                var result = await userManager.CreateAsync(userAdmin, password);

                // Si la creación del usuario fue exitosa, se le asigna el rol de ADMIN
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userAdmin, SystemRole.ADMIN);
                }
            }
        }
    }
}