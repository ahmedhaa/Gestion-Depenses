using Gestion_Dépenses.Models.UserModel;
using Microsoft.AspNetCore.Identity;

namespace Gestion_Dépenses.Services
{
    public class Initializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User" };

            // Créer les rôles si ils n'existent pas 
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Créer l'utilisateur Admin
            var user = await userManager.FindByEmailAsync("hitech@test.com");
            if (user == null)
            {
                var adminUser = new User
                {
                    UserName = "hitech",
                    Email = "hitech@test.com"
                };

                var createUser = await userManager.CreateAsync(adminUser, "Password123!");
                if (createUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    foreach (var error in createUser.Errors)
                    {
                        Console.WriteLine($"Erreur : {error.Code} - {error.Description}");
                    }
                }
            }
        }
    }
}
