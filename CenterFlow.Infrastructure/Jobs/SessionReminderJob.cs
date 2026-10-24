using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CenterFlow.Infrastructure.Jobs
{
    public class SessionReminderJob : ISessionReminderJob
    {
        private readonly IAppDbContext context;
        private readonly INotificationService notificationService;
        private readonly ILogger<SessionReminderJob> logger;
        public SessionReminderJob (  IAppDbContext context
            , INotificationService notificationService
            ,ILogger<SessionReminderJob>logger)
        {
            this.notificationService = notificationService;
            this.context = context;
            this.logger = logger;
        }
        public async Task SendAsync(Guid bookId)
        {
            var book = await context.Books
                .Where(x => x.Id == bookId && x.Status != Domain.Enum.BookingStatus.Cancelled)
                .Select(x=>new {x.TeacherId, x.Teacher.FullName,x.From})
                .FirstOrDefaultAsync();
            var studentBook = await context.StudentBookings
                .Where(x => x.BookId == bookId && x.Status != Domain.Enum.StudentBookingStatus.Cancelled)
                .Select(x => new
                {
                    x.StudentId,
                    x.Student.FullName,
                   
                }).ToListAsync();
            if (book == null) return;
            
            string body = $"Hello {book.FullName}, just a reminder that your session starts at {book.From} in 30 minutes. Have a great session!";

         await SafeSend(book.TeacherId.ToString ( )  , "Session Reminder", body, bookId);
            foreach (var i in studentBook)
            {
                string title = "Class Reminder";
                string studentBody = $"Hello {i.FullName}, your session with {book.FullName} starts at {book.From} in 30 minutes. Get ready and good luck!";
                await SafeSend(i.StudentId.ToString(), title, body, bookId);

            }
        }
        private async Task SafeSend(string userId, string title, string body, Guid bookId)
        {
            try
            {
                await notificationService.SendAsync(userId, title, body,
                    NotificationType.SessionReminder, bookId.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
       "Failed to send session reminder to user {UserId} for book {BookId}",
       userId, bookId);
            }
        }
    }
}
