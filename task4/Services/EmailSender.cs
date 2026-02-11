using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Text;

namespace task4.Services
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
    }

    public class EmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;

        public EmailSender(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // important: send email asynchronously via SMTP
            using var client = new System.Net.Mail.SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.EnableSsl,
                Credentials = new System.Net.NetworkCredential(_options.User, _options.Password)
            };

            var mail = new System.Net.Mail.MailMessage()
            {
                From = new System.Net.Mail.MailAddress(_options.User, "Task4 Support"),
                Subject = subject,
                IsBodyHtml = true,
            };
            mail.To.Add(email);

            // create a plain-text fallback by stripping tags (simple but effective for most cases)
            var plainText = Regex.Replace(htmlMessage ?? string.Empty, "<[^>]+>", string.Empty);

            // add alternate views for clients that prefer plain text
            var plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(plainText, Encoding.UTF8, "text/plain");
            var htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(htmlMessage ?? string.Empty, Encoding.UTF8, "text/html");
            mail.AlternateViews.Add(plainView);
            mail.AlternateViews.Add(htmlView);

            // set a default body (some clients read Body directly)
            mail.Body = plainText;

            // ensure UTF-8 headers
            mail.HeadersEncoding = Encoding.UTF8;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;

            await client.SendMailAsync(mail);
        }
    }
}
