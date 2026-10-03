public interface INotificationService
{
    Task SendAsync(
        string userId,
        string title,
        string body,
        NotificationType type,
        string? referenceId = null,
        CancellationToken cancellationToken = default);
        }