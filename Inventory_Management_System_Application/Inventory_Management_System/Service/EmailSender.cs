using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using Inventory_Management_System.Service;
using WebPWrecover.Services;


namespace Inventory_Management_System.Service
{

    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly AuthMessageSenderOptions _options;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            _options = optionsAccessor.Value;
            _logger = logger;

            _logger.LogInformation($"SendGrid API Key from Options: {_options?.SendGridKey}");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            
            try
            {
                if (string.IsNullOrEmpty(_options?.SendGridKey))
                {
                    _logger.LogError("SendGrid API key is null or empty.");
                    return;
                }

                await Execute(_options.SendGridKey, subject, message, toEmail);
                _logger.LogInformation($"Email to {toEmail} queued successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {toEmail}: {ex.Message}");
            }
        }

        private async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("waremaster2024@gmail.com", "NoReply"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failure sending email to {toEmail}. Status code: {response.StatusCode}");
            }
        }
    }

}