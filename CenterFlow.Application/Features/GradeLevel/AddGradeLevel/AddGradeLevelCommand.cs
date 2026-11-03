
using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.GradeLevel.AddGradeLevel;

public record AddGradeLevelCommand(string Name) : IRequest<Response<Guid>>;
