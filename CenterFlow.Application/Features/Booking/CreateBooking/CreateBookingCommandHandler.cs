
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RedLockNet;
using RedLockNet.SERedis;

namespace CenterFlow.Application.Features.Booking.CreateBooking
{
    public class CreateBookingCommandHandler
        : IRequestHandler<CreateBookingCommand, Response<Guid>>
    {
        private readonly IAppDbContext context;
        private INotificationService notificationService;
        private readonly ICurrentUserService userService;
        private readonly IDistributedLockFactory redLockFactory;
        public CreateBookingCommandHandler(IAppDbContext context
            ,ICurrentUserService userService
            , IDistributedLockFactory redLockFactory
            ,INotificationService notificationService)
        {
            this.context = context;
            this.userService = userService;
            this.redLockFactory = redLockFactory;
            this.notificationService = notificationService;
        }
        public async Task<Response<Guid>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var teacher = await context.Teachers.FirstOrDefaultAsync(x => x.Id == userService.UserId);

            var roomId = await context.Rooms.FirstOrDefaultAsync(x => x.Id == request.RoomId);
            if (teacher == null || roomId==null)
                throw new NotFoundException("data Not found");

            var key = $"booking-lock:room:{request.RoomId}:{request.date:yyyyMMdd}:{teacher.Id}";
            var expiry = TimeSpan.FromSeconds(10);
            await using var redLock = await redLockFactory.CreateLockAsync(
                resource:key,
                expiryTime:TimeSpan.FromSeconds(10),
                waitTime:TimeSpan.FromSeconds (2),
                retryTime:TimeSpan.FromMilliseconds(200)
                );
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
                && (x.RoomId == request.RoomId || x.Teacher.Id == teacher.Id)
                && x.From < request.To
                && x.To > request.From);

            if (booking)
                throw new ConflictException("This room has been booked");
            if (isConflict)
                throw new ConflictException("You already have a booking during this time.");
            var book = new Book()
            {
                Date = request.date,
                From = request.From,
                Id = Guid.NewGuid(),
                RoomId = request.RoomId,
                Status = Domain.Enum.BookingStatus.Confirmed,
                TeacherId = Guid.Parse(userService.UserId),
                To = request.To
            };
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();
            await notificationService.SendAsync(
    userId: userService.UserId.ToString(),
    title: "New Booking",
    body: $"You booked a session on {request.date}.",
    type: NotificationType.BookingCreated,
    referenceId: book.Id.ToString(),
    cancellationToken: cancellationToken);
            return new Response<Guid>()
            {
                Data = book.Id,
                Message = "Room has been booked successfully",
                Success = true
            };
        }
    }
}
