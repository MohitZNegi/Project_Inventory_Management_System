using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Inventory_Management_System.Models;

public class ContactController : Controller
{
    private readonly IEmailSender _emailSender;

    public ContactController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
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
            string subject = $"New Contact Form Submission from {model.FirstName} {model.LastName}";
            string message = $"First Name: {model.FirstName}\n" +
                             $"Last Name: {model.LastName}\n" +
                             $"Email: {model.Email}\n" +
                             $"Phone: {model.Phone}\n" +
                             $"Topic: {model.Topic}\n" +
                             $"Message: {model.Message}";

            await _emailSender.SendEmailAsync("camerondalhousie@gmail.com", subject, message);
            ViewData["Message"] = "Your message has been sent successfully!";
            return View();
        }

        return View(model);
    }
}
