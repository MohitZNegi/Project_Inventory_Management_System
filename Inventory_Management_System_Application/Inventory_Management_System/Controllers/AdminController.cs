using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Inventory_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        /*  public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
          {
              _userManager = userManager;
              _roleManager = roleManager;
              _dbContext = dbContext;
          }

          [HttpGet]
          public IActionResult CreateUser()
          {
              return View();
          }

          [HttpPost]
          public async Task<IActionResult> CreateUser(ClientView model)
          {
              if (ModelState.IsValid)
              {
                  var user = new ApplicationUser
                  {
                      UserName = model.Email,
                      Email = model.Email,
                      EmailConfirmed = true // Set to true if you don't want email confirmation
                  };

                  var result = await _userManager.CreateAsync(user, model.Password);

                  if (result.Succeeded)
                  {
                      // Assign the user to the appropriate role
                      await _userManager.AddToRoleAsync(user, "Client");

                      // Create a new Client record
                      var client = new Client
                      {
                          Client_Id = Guid.NewGuid().ToString(),
                          User = user,
                          First_name = model.FirstName,
                          Last_name = model.LastName,
                          Email = model.Email
                      };

                      _dbContext.Client_Model.Add(client);
                      await _dbContext.SaveChangesAsync();


                      foreach (var error in result.Errors)
                      {
                          ModelState.AddModelError(string.Empty, error.Description);
                      }
                  }

                  return View(model);
              }
          }*/
    }
}