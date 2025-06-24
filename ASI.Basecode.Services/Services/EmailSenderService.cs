using ASI.Basecode.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class EmailSenderService:IEmailSender
    {
        //To communicate with the appjson
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private bool IsValidEmailFormat(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email; // Ensure no extra parts if needed
            }
            catch
            {
                return false;
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"]);
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];



            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fromEmail))
            {
                throw new InvalidOperationException("SMTP settings are not fully configured.");
            }

            if (!IsValidEmailFormat(toEmail))
            {
                throw new ArgumentException("The 'toEmail' address is not in a valid format.");
            }
            if (!IsValidEmailFormat(fromEmail))
            {
                throw new InvalidOperationException("The configured 'FromEmail' address is not in a valid format.");
            }

            try
            {
                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;
                    client.UseDefaultCredentials = false; // Important: set to false when providing explicit credentials
                    client.Credentials = new NetworkCredential(username, password);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true // Assuming your OTP will be in HTML
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine($"OTP to {toEmail} sent via SMTP successfully.");
                }
            }
            catch (SmtpException ex)
            {
                throw new Exception("Failed to send email via SMTP. Check SMTP settings and credentials");
            }
            catch (FormatException ex)
            {
                throw new Exception($"Failed to send email due to an invalid email address format");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
