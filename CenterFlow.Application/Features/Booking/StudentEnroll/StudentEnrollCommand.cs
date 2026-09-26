using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Booking.StudentEnroll;

public record StudentEnrollCommand(Guid BookId) : IRequest<Response<Guid>>
;