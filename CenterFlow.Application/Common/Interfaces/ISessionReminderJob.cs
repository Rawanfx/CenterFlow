namespace CenterFlow.Application.Common.Interfaces
{
    public interface ISessionReminderJob
    {
        Task SendAsync(Guid bookId);
    }
}
