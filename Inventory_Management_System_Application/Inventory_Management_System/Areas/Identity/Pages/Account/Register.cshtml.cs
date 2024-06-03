// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Inventory_Management_System.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Inventory_Management_System.Models;
using Inventory_Management_System.Constants;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)

        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        private string GenerateTemporaryPassword(string email)
        {
            // Split the email into words
            var words = email.Split('@', '.').SelectMany(w => w.Split('_')).Where(w => !string.IsNullOrWhiteSpace(w)).ToList();

            // Shuffle the words
            var random = new Random();
            var shuffledWords = words.OrderBy(w => random.Next()).ToList();

            // Take a subset of words to form the password (adjust the length as needed)
            var passwordWords = shuffledWords.Take(3); // Adjust the number of words as needed

            // Combine the words to form the password
            var temporaryPassword = string.Join("", passwordWords);

            // Add a non-alphanumeric character to the password
            var nonAlphanumericChars = "!@#$%^&*()-_+=?";
            temporaryPassword += nonAlphanumericChars[random.Next(nonAlphanumericChars.Length)];

            // Add an uppercase character to the password
            var uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            temporaryPassword += uppercaseChars[random.Next(uppercaseChars.Length)];

            return temporaryPassword;
        }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>



        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // Generate a temporary password based on the email
                var temporaryPassword = GenerateTemporaryPassword(Input.Email);

                var result = await _userManager.CreateAsync(user, temporaryPassword);


                if (result.Succeeded)
                {
                    // Assign the user to the "Client" role by default
                    await _userManager.AddToRoleAsync(user, Roles.Client.ToString());



                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm Your Email",
                    $"Dear User," +
                    $"<br><br>" +
                    $"Welcome to Waremaster! \n" +
                    $" We're delighted to have you onboard. \n" +
                    $" An account has been created for you. Please confirm your email address by clicking <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>. \n" +
                    $" As part of the verification process, here's your temporary password: <strong>{temporaryPassword}</strong>. \n" +
                    $" Once you've confirmed your email, please log in using this temporary password and change it immediately for security reasons." +
                    $"<br><br>" +
                    $"Best regards," +
                    $"<br><br>" +
                    $"The Waremaster Team");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        TempData["ConfirmEmailMessage"] = "A confirmation email has been sent to your email address. Please confirm your email before logging in.";
                        return RedirectToPage("Login", new { returnUrl = returnUrl });
                    }
                    else
                    {
                        TempData["ConfirmEmailMessage"] = "Registration successful. A confirmation email has been sent to your email address. Please confirm your email before logging in.";
                        return RedirectToPage("Login", new { returnUrl = returnUrl });
                    }

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}