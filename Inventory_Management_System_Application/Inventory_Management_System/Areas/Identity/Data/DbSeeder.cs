using System;

using System.Collections.Generic;

using System.Data;

using System.Linq;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Inventory_Management_System.Constants;


using Microsoft.VisualBasic;

namespace Inventory_Management_System.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class

public class DbSeeder

{

    public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
    {
        var userManager = service.GetService<UserManager<ApplicationUser>>();
        var roleManager = service.GetService<RoleManager<IdentityRole>>();
        await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Client.ToString()));


        // Creating admin
        var user = new ApplicationUser
        {
            UserName = "waremaster2024@gmail.com",
            Email = "waremaster2024@gmail.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
        };

        var userInDb = await userManager.FindByEmailAsync(user.Email);
        if (userInDb == null)
        {
            await userManager.CreateAsync(user, "Password-1");
            await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
        }
    }

}