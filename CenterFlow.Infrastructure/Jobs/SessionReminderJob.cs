
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CenterFlow.Infrastructure.Jobs
{
    public class SessionReminderJob : ISessionReminderJob
    {
        private readonly IAppDbContext context;
        private readonly INotificationService notificationService;
        private SessionReminderSetting setting;
        public SessionReminderJob(IAppDbContext context
            ,INotificationService notificationService
            ,IOptions<SessionReminderSetting> options)
        {
            this.context = context;
            this.notificationService = notificationService;
            setting = options.Value;
        }
        public async Task Exceute()
        {
            var books = await context.Books
                .Where(x => x.Status != Domain.Enum.BookingStatus.Cancelled
                && x.Date == DateOnly.FromDateTime(DateTime.UtcNow)
                && x.ReminderSentAt != DateTime.UtcNow
                )
                .ToListAsync();
            foreach (var i in books)
            {
                i.ReminderSentAt = DateTime.UtcNow;
                await notificationService.SendAsync(i.TeacherId.ToString(), "Session Reminder", "", NotificationType.SessionReminder);
                foreach (var j in i.StudentBookings) 
                await notificationService.SendAsync(j.StudentId.ToString(), "Session Reminder", "", NotificationType.SessionReminder)
;
            }
            await context.SaveChangesAsync();
        }
    }
}
