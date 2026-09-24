
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Booking.CreateBooking
{
    public class CreateBookingCommandHandler
        : IRequestHandler<CreateBookingCommand, Response<Guid>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public CreateBookingCommandHandler(IAppDbContext context,ICurrentUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<Guid>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var teacher = await context.Teachers.FirstOrDefaultAsync(x => x.Id == userService.UserId);

            var roomId = await context.Rooms.FirstOrDefaultAsync(x => x.Id == request.RoomId);
            if (teacher == null || roomId==null)
                throw new NotFoundException("data Not found");

            var isWithinAvailability = await context.TeacherAvailabilities
          .AnyAsync(a => a.TeacherId ==Guid.Parse( teacher.Id) 
          && a.DayOfWeek == request.date.DayOfWeek
                && a.From <= request.From
                && a.To >= request.To);

            if (!isWithinAvailability)
                throw new InvalidBooking("This time is outside your declared availability.");

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
                && (x.RoomId == request.RoomId || teacher.Id == teacher.Id)
                && x.From < request.To
                && x.To > request.From);
            if (booking)
                throw new InvalidBooking("This room has been booked");
            if (isConflict)
                throw new InvalidBooking("You already have a booking during this time.");
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
            return new Response<Guid>()
            {
                Data = book.Id,
                Message = "Room has been booked successfully",
                Success = true
            };
        }
    }
}
