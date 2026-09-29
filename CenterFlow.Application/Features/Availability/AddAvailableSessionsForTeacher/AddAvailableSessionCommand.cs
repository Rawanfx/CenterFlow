using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Availability.AddAvailableSessionsForTeacher;

public record AddAvailableSessionCommand(DayOfWeek Day, TimeSpan From, TimeSpan To) : IRequest<Response<Guid>>;

