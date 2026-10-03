using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Enrollment.StudentEnroll;

public record StudentEnrollCommand(Guid BookId) : IRequest<Response<Guid>>
;