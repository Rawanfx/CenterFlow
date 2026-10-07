using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Availability.GetAvailableSlots
{
    public class GetAvailableSessionsQueryHandler
        : IRequestHandler<GetAvailableSessionsQuery, Response<List<GetAvailableSessionsDto>>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public async Task<Response<List<GetAvailableSessionsDto>>> Handle(GetAvailableSessionsQuery request, CancellationToken cancellationToken)
        {

            var availableBooking = await context.Books
                 .Where(x => x.TeacherId == request.TeacherId
                 && x.Status != Domain.Enum.BookingStatus.Cancelled
                 && x.Date == request.date
                 && x.StudentBookings.Count(u => u.Status!=Domain.Enum.StudentBookingStatus.Cancelled) + 1 <= x.Room.Capacity)
                 .AsNoTracking()
                 .Select(y=> new GetAvailableSessionsDto()
                 {
                     AvaliableSeats = y.Room.Capacity-y.StudentBookings.Count(u => u.Status!=Domain.Enum.StudentBookingStatus.Cancelled),
                     BookId=y.Id,
                     From = y.From,
                     To = y.To,
                     RoomName=y.Room.Name
                 }).ToListAsync();
            return new Response<List<GetAvailableSessionsDto>>()
            {
                Data = availableBooking,
                Success =true
            };
                
        }
    }
}
