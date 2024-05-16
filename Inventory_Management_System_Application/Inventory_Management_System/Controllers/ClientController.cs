using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Inventory_Management_System.ViewModels;
using SendGrid.Helpers.Mail;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Inventory_Management_System.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public ClientController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult ViewProfile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            var profile = _dbContext.Client_Model.Where(e => e.Client_Id == userId).FirstOrDefault();


            if (profile == null)
            {
                return RedirectToAction("Create", "Client");
            }

            var viewModel = new Client
            {

                First_name = profile.First_name,
                Last_name = profile.Last_name,


            };

            return View(viewModel);




        }
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var existingProfile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (existingProfile != null)
            {
                return Content("You have already created a profile!");
            }

            var viewModel = new ClientView();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientView clientVM)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var existingProfile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (ModelState.IsValid && existingProfile == null)
            {
                var profile = new Client
                {
                    UserId = userId,
                    Client_Id = userId,
                    First_name = clientVM.FirstName,
                    Last_name = clientVM.LastName,
                    // Add other properties
                };

                await _dbContext.Client_Model.AddAsync(profile);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(clientVM);
        }

        public IActionResult Update()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var profile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (profile == null)
            {
                return RedirectToAction("Create", "Client");
            }

            var viewModel = new ClientView
            {
                FirstName = profile.First_name,
                LastName = profile.Last_name,

                // Add other properties
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAsync(ClientView clientVM)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var profile = _dbContext.Client_Model.FirstOrDefault(c => c.UserId == userId);

            if (ModelState.IsValid && profile != null)
            {
                profile.First_name = clientVM.FirstName;
                profile.Last_name = clientVM.LastName;
                // Update other properties

                _dbContext.Update(profile);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(clientVM);
        }

        
    }
}