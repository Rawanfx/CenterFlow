
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Application.Common.Models.OutboxMessage;
using CenterFlow.Domain.Entities;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RedLockNet;
using RedLockNet.SERedis;
using System.Diagnostics;

namespace CenterFlow.Application.Features.Booking.CreateBooking
{
    public class CreateBookingCommandHandler
        : IRequestHandler<CreateBookingCommand, Response<Guid>>
    {
        private readonly IAppDbContext context;
        private INotificationService notificationService;
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly ICurrentUserService userService;
        private readonly IDistributedLockFactory redLockFactory;
        public CreateBookingCommandHandler(IAppDbContext context
            ,ICurrentUserService userService
            , IDistributedLockFactory redLockFactory
            ,INotificationService notificationService
            ,IBackgroundJobClient backgroundJobClient)
        {
            this.context = context;
            this.userService = userService;
            this.redLockFactory = redLockFactory;
            this.notificationService = notificationService;
            this.backgroundJobClient = backgroundJobClient;
        }
        public async Task<Response<Guid>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var teacher = await context.Teachers.FirstOrDefaultAsync(x => x.Id == userService.UserId);

            var roomId = await context.Rooms.FirstOrDefaultAsync(x => x.Id == request.RoomId);
            if (teacher == null || roomId==null)
                throw new NotFoundException("data Not found");

            var key = $"booking-lock:room:{request.RoomId}:{request.date:yyyyMMdd}";
            var expiry = TimeSpan.FromSeconds(10);
            var stopwatch = Stopwatch.StartNew();

            await using var redLock = await redLockFactory.CreateLockAsync(
                resource:key,
                expiryTime:TimeSpan.FromSeconds(10),
                waitTime:TimeSpan.FromSeconds (2),
                retryTime:TimeSpan.FromMilliseconds(200)
                );
            stopwatch.Stop();
            Console.WriteLine(
                $"Lock acquisition took: {stopwatch.ElapsedMilliseconds} ms");
            if (!redLock.IsAcquired)
                throw new ConflictException("Someone else is booking this room right now, please try again.");

            var isWithinAvailability = await context.TeacherAvailabilities
          .AnyAsync(a => a.TeacherId ==Guid.Parse( teacher.Id) 
          && a.DayOfWeek == request.date.DayOfWeek
                && a.From <= request.From
                && a.To >= request.To && !a.IsDelete);

            if (!isWithinAvailability)
                throw new ConflictException("This time is outside your declared availability.");

            var booking = await context.Books
                .AnyAsync(x => x.Status != Domain.Enum.BookingStatus.Cancelled
                && x.Date.Year == request.date.Year
                && x.Date.Month == request.date.Month
                && x.Date.Day == request.date.Day
               && x.From < request.To
                && x.To > request.From
                && x.RoomId == request.RoomId);

            var isConflict = await context.Books
    .AnyAsync(x => x.Status != Domain.Enum.BookingStatus.Cancelled
                && x.Date == request.date
                && (x.RoomId == request.RoomId || x.TeacherId ==Guid.Parse( teacher.Id))
                && x.From < request.To
                && x.To > request.From);

            if (booking)
                throw new ConflictException("This room has been booked");
            if (isConflict)
                throw new ConflictException("You already have a booking during this time.");

            var teacherAssign = await context.TeacherSubjectAssignment
                .FirstOrDefaultAsync(x => x.Id == request.TeacherSubjectAssign && x.TeacherId == teacher.Id);
            if (teacherAssign == null)
                throw new NotFoundException("Invalid Data");
            var book = new Book()
            {
                Date = request.date,
                From = request.From,
                Id = Guid.NewGuid(),
                RoomId = request.RoomId,
                Status = Domain.Enum.BookingStatus.Confirmed,
                TeacherId = Guid.Parse(userService.UserId),
                To = request.To,
                TeacherSubjectAssignId=request.TeacherSubjectAssign
            };
            await context.Books.AddAsync(book);
            
            var notificationEvent = new BookingNotificationEvent
            {
                BookingId = book.Id,
                UserId = userService.UserId.ToString(),
                Title = "New Booking",
                Body = $"You booked a session on {request.date}.",
                Type = NotificationType.BookingCreated,
                ReferenceId = book.Id.ToString(),
               
            };

            var messages = new List<Outbox>()
            {
                new Outbox
                {
                    Id = Guid.NewGuid(),
                    Type = nameof(BookingNotificationEvent),
                    Content = System.Text.Json.JsonSerializer.Serialize(notificationEvent),
                    OccurredOnUtc = DateTime.UtcNow,
                    RetryCount = 0
                }
            };

            var tz = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var local = book.Date.ToDateTime(TimeOnly.FromTimeSpan(book.From)).AddMinutes(-30);
            var runAt = new DateTimeOffset(local, tz.GetUtcOffset(local));
            if (runAt > DateTimeOffset.UtcNow)
            {
               // backgroundJobClient.Schedule<ISessionReminderJob>(x => x.SendAsync(book.Id), runAt);
                var reminderEvent = new BookingReminderEvent()
                {
                    BookId = book.Id,
                    RunAt = runAt.UtcDateTime
                };
                messages.Add(new Outbox()
                {
                    Id = Guid.NewGuid(),
                    Type = nameof(BookingReminderEvent),
                    Content = System.Text.Json.JsonSerializer.Serialize(reminderEvent),
                    OccurredOnUtc = DateTime.UtcNow,
                    RetryCount = 0
                });
            }
            await context.OutboxMessage.AddRangeAsync(messages);
            await context.SaveChangesAsync();
            return new Response<Guid>()
            {
                Data = book.Id,
                Message = "Room has been booked successfully",
                Success = true
            };
        }
        
    }
}
