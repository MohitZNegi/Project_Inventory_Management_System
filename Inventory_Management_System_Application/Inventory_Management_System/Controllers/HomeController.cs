using Inventory_Management_System.Areas.Identity.Data;
using Inventory_Management_System.Models;
using Inventory_Management_System.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Inventory_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailSender _emailSender;

        public HomeController(ApplicationDbContext dbContext, ILogger<HomeController> logger, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve products from the database
            var products = await _dbContext.Product_Model.ToListAsync();

            // Pass the productViews list to the view
            return View(products);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Product()
        {
            // Retrieve products from the database
            var products = await _dbContext.Product_Model.ToListAsync();

            // Pass the productViews list to the view
            return View(products);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ContactUs(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                string subject = $"Contact Form from {model.FirstName} {model.LastName}";
                string message = $"Name: {model.FirstName} {model.LastName}\n" +
                                 $"Email: {model.Email}\n" +
                                 $"Phone: {model.Phone}\n" +
                                 $"Topic: {model.Topic}\n" +
                                 $"Message: {model.Message}";

                await _emailSender.SendEmailAsync("waremaster2024@gmail.com", subject, message);
                ViewData["Message"] = "Your message has been sent successfully!";
                return View();
            }

            return View(model);
        }

        public IActionResult TermsAndServices()
        {
            return View();

        }

        public IActionResult FAQs()
        {
            return View();

        }

        [HttpGet]
        [Route("Home/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/Register")]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                string subject = $"Account Request from {model.FirstName} {model.LastName}";
                string message = $"Name: {model.FirstName} {model.LastName}\n" +
                                 $"Email: {model.Email}\n" +
                                 $"Phone: {model.Phone}\n" +
                                 $"Business Name: {model.Business}\n" +
                                 $"NZBN: {model.nzbn}";

                await _emailSender.SendEmailAsync("waremaster2024@gmail.com", subject, message);
                ViewData["Message"] = "Account Request Sent";
                return View();
            }

            return View(model);
        }

    }
}