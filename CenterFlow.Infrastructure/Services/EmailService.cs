using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using CenterFlow.Application.Common.Models;

namespace CenterFlow.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting emailSetting;
        public EmailService(IOptions<EmailSetting> options)
        {
            emailSetting = options.Value;
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
