using CenterFlow.Application.Common.Models;

namespace CenterFlow.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendBookingCancelledEmailAsync(
          SendEmailDto dto
            );
    }
}
