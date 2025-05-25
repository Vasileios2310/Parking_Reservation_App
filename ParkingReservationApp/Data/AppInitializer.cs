using Microsoft.AspNetCore.Identity;
using ParkingReservationApp.Models;

namespace ParkingReservationApp.Data;

public class AppInitializer
{

    public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        string[] roles = { "Admin", "User" , "Manager" };
        
        foreach (var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole {Name = role});
        }

        var adminEmail = "admin@admin.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if(adminUser == null)
        {
            var user = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                Firstname = "Super",
                Lastname = "Admin",
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(user, "Admin123!");
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}