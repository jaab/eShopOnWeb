using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
     
       private readonly IOptions<EmailConfig> _configOptions;

        public EmailSender(IOptions<EmailConfig> configOptions)
        {
            this._configOptions = configOptions;
        }

       /* private readonly EmailConfig _emailConfig = new EmailConfig();

        public EmailSender(IConfiguration config)
        {
            _emailConfig.SmtpServer = config.GetValue<string>("SmtpServer");
            _emailConfig.SmtpUsername = config.GetValue<string>("SmtpUsername");
            _emailConfig.SmtpPassword = config.GetValue<string>("SmtpPassword");
            _emailConfig.SmtpPort = config.GetValue<int>("SmtpPort");
        }*/

        private async Task SendEmailAsync(string email, string subject, string body, bool isHtml = false)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configOptions.Value.SmtpUsername));
            message.To.Add(new MailboxAddress(email));
            message.Subject = subject;

            var textFormat = isHtml ? TextFormat.Html : TextFormat.Plain;
            message.Body = new TextPart(textFormat)
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                // Accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                await client.ConnectAsync(_configOptions.Value.SmtpServer, _configOptions.Value.SmtpPort, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
               await client.AuthenticateAsync(_configOptions.Value.SmtpUsername, _configOptions.Value.SmtpPassword);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return SendEmailAsync(email, subject, message, true);
        }
    }
}