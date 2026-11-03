using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.Subjects.AddSubjects;

public record AddSubjectCommand(string Name) :
    IRequest<Response<Guid>>;
