
using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Booking.CreateBooking;

public record CreateBookingCommand( Guid RoomId, TimeSpan From, TimeSpan To, DateTime date)
    : IRequest<Response<Guid>>;

