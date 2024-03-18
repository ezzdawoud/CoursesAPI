using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using Courses.Helper;

namespace Courses.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly EmailOptions _emailOptions;
        private readonly ISendGridClient _sendGridClient;

        public EmailSenderService(ISendGridClient sendGridClient, IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
            _sendGridClient = sendGridClient;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_emailOptions.FromEmail, _emailOptions.EmailName),
                Subject = subject,
                HtmlContent = htmlMessage
            };
            msg.AddTo(email);
            await _sendGridClient.SendEmailAsync(msg);
        }
    }
}
