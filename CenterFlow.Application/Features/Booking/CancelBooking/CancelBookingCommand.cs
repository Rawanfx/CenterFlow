using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Booking.CancelBooking;

public record CancelBookingCommand(Guid BookId) : IRequest<Response<string>>;

