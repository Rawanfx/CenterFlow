using CenterFlow.Application.Common.Models;

namespace CenterFlow.Application.Common.Interfaces
{
    public interface ICancelEnrollment
    {
        Task BookingCancellationNotifier(List<SendEmailDto> dto);
    }
}
