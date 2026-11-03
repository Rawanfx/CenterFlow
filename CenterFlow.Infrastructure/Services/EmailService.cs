using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MimeKit;

namespace CenterFlow.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting emailSetting;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly AppConfig appConfig;
        public EmailService(IOptions<EmailSetting> options
            , IOptions<AppConfig> appConfigOptions
            ,IWebHostEnvironment webHostEnvironment)
        {
            emailSetting = options.Value;
            appConfig = appConfigOptions.Value;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task ConfirmEmail(string email, string token)
        {
            string fileName = "ConfirmationEmail.html";
            string replaceWord = "confirm-email";
            string baseUrl = appConfig.AppUrl;
            var confirmationLink = $"{baseUrl}/api/Auth/confirm-email?email={email}&token={token}";
            string path = Path.Combine(webHostEnvironment.WebRootPath,  fileName);
            string content = await File.ReadAllTextAsync(path);
            string finalContent = content.Replace(replaceWord, confirmationLink);
            await SendEmailAsync(mailTo: email, subject: "Center Flow", null, finalContent);

        }
        public async Task SendEmailAsync(string mailTo, string? subject, List<IFormFile> attachments, string body)
        {
            var message = new MimeMessage();

            // From
            message.From.Add(new MailboxAddress("Center Flow", emailSetting.SenderEmail));

            // To
            message.To.Add(new MailboxAddress("", mailTo));

            message.Subject = subject ?? string.Empty;

            var builder = new BodyBuilder();

            // Attachments
            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        builder.Attachments.Add(file.FileName, ms.ToArray());
                    }
                }
            }


            builder.HtmlBody = body;

            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(emailSetting.SmtpServer, emailSetting.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(emailSetting.SenderEmail, emailSetting.SenderPassword);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }


        public async Task SendBookingCancelledEmailAsync(SendEmailDto dto
            )
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(emailSetting.SenderName, emailSetting.SenderEmail));
            message.To.Add(new MailboxAddress(dto.studentName,dto. studentEmail));
            message.Subject = "Booking Cancelled";
            message.Body = new TextPart("plain")
            {
                Text = $"Hi {dto.studentName},\n\n" +
                       $"Your session with {dto.teacherName} on {dto.date} from {dto.from} to {dto.to} has been cancelled.\n\n" +
                       $"We're sorry for the inconvenience."
            };
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailSetting.SmtpServer, emailSetting.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailSetting.SenderEmail, emailSetting.SenderPassword);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
