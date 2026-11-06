using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models.OutboxMessage;
using CenterFlow.Domain.Entities;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RedLockNet;

namespace CenterFlow.Application.Common.Service
{
    public class OutboxProcessor
    {
        private readonly INotificationService notificationService;
        private readonly IAppDbContext context;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IDistributedLockFactory distributedLockFactory;
        public OutboxProcessor (INotificationService notificationService, IAppDbContext context, IBackgroundJobClient backgroundJobClient, IDistributedLockFactory distributedLockFactory)
        {
            this.notificationService = notificationService;
            this.context = context;
            this.backgroundJobClient = backgroundJobClient;
            this.distributedLockFactory = distributedLockFactory;
        }
        public async Task Process()
        {
            await using var redLock = await distributedLockFactory.CreateLockAsync(
                resource: "outbox-processor-lock",
                expiryTime: TimeSpan.FromSeconds(30),
                waitTime: TimeSpan.FromSeconds(5),
                retryTime: TimeSpan.FromMilliseconds(200)
            );
            var messages = await context.OutboxMessage
                .Where(x => x.ProcessedOnUtc == null)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(20).ToListAsync();
            foreach (var message in messages)
            {
                try
                {
                    if (message.Type == nameof(BookingNotificationEvent))
                        await ProcessNotificationEvent(message);
                    else if (message.Type== nameof(BookingReminderEvent))
                        await ProcessReminderEvent(message);
                }
                catch(Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;
                }
                await context.SaveChangesAsync();
            }
             
        }
        private async Task ProcessNotificationEvent (Outbox message)
        {
            var notificationEvent = System.Text.Json.JsonSerializer.Deserialize<BookingNotificationEvent>(message.Content);
            if (notificationEvent != null)
            {
                await notificationService.SendAsync(notificationEvent.UserId, notificationEvent.Title, notificationEvent.Body, notificationEvent.Type, notificationEvent.ReferenceId);
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            else 
                throw new InvalidCastException("Invalid notification event");
        }
        private async Task ProcessReminderEvent (Outbox message)
        {
            var reminderEvent = System.Text.Json.JsonSerializer.Deserialize<BookingReminderEvent>(message.Type
                )?? throw new InvalidCastException("Invalid reminder event");
            if (reminderEvent.RunAt>= DateTime.UtcNow)
            backgroundJobClient.Schedule<INotificationService>(x => x.SendAsync(reminderEvent.BookId.ToString(), "Booking Reminder", $"You have a booking scheduled at {reminderEvent.RunAt}.", NotificationType.SessionReminder, reminderEvent.BookId.ToString()), reminderEvent.RunAt);
        }
    }
}
