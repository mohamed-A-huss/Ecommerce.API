using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace Ecommerce.API.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("momon2516@gmail.com", "yzvp wztc dtjx hbrt")
            };

            return client.SendMailAsync(
                new MailMessage(from: "momon2516@gmail.com",
                                to: email,
                                subject,
                                htmlMessage
                                )
                {
                    IsBodyHtml = true
                });
        }
    }
}
