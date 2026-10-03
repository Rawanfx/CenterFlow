using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
namespace CenterFlow.Infrastructure.Services
{
    public class CancelEnrollment : ICancelEnrollment
    {
        private readonly IEmailService emailService;
        public CancelEnrollment(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        public async Task BookingCancellationNotifier(List<SendEmailDto> dto)
        {
            foreach (var i in dto)
                await emailService.SendBookingCancelledEmailAsync(i);
        }
    }
}
