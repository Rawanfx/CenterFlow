using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Booking.CancelEnrollment;

public record CancelEnrollmentCommand(Guid EnrollId) : IRequest<Response<string>>;  
