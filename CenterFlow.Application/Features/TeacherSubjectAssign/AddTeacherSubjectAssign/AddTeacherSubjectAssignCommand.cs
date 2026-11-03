using CenterFlow.Application.Common.Models;
using MediatR;

namespace CenterFlow.Application.Features.TeacherSubjectAssign.AddTeacherSubjectAssign;

public record AddTeacherSubjectAssignCommand(Guid SubjectId, Guid GradeLevelId)
    : IRequest<Response<Guid>>;
