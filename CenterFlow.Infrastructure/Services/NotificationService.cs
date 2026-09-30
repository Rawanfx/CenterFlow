using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;
using CenterFlow.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IAppDbContext context;
        private readonly IHubContext<NotificationHub> hubContext;
        public NotificationService(IAppDbContext context, IHubContext<NotificationHub> hubContext)
        {
            this.context = context;
            this.hubContext = hubContext;
        }
        public async Task SendAsync(string userId, string title, string body, NotificationType type, string? referenceId = null, CancellationToken cancellationToken = default)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Body = body,
                Type = type,
                ReferenceId = referenceId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            context.Notifications.Add(notification);
            await context.SaveChangesAsync(cancellationToken);
            await hubContext.Clients.User(userId)
                .SendAsync("ReceiveNotification", new
                {
                    notification.Id,
                    notification.Title,
                    notification.Body,
                    notification.Type,
                    notification.CreatedAt
                },cancellationToken);
        }
    }
}
