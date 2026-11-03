using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.TeacherSubjectAssign.AddTeacherSubjectAssign
{
    public class AddTeacherSubjectAssignCommandHandler : IRequestHandler<AddTeacherSubjectAssignCommand, Response<Guid>>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IAppDbContext context;
        public AddTeacherSubjectAssignCommandHandler(ICurrentUserService currentUserService, IAppDbContext context)
        {
            this.currentUserService = currentUserService;
            this.context = context;
        }
        public async Task<Response<Guid>> Handle(AddTeacherSubjectAssignCommand request, CancellationToken cancellationToken)
        {
            var teacher = await context.Teachers.SingleAsync(x => x.Id == currentUserService.UserId);
            var subject = await context.Subjects.SingleAsync(x => x.Id == request.SubjectId);
            var gradeLevel = await context.GradeLevels.SingleAsync(x => x.Id == request.GradeLevelId);

            var teacherSubjectAssign = await context.TeacherSubjectAssignment
                .FirstOrDefaultAsync(x => x.GradeLevelId == request.GradeLevelId
                && x.IsActive
                && x.SubjectId == request.SubjectId
                && x.TeacherId == teacher.Id);
            if (teacherSubjectAssign != null)
            {
                return new Response<Guid>
                {
                    Success = false,
                    Message = "This subject is already assigned to you for this grade level"
                };
            }
            var newTeacherSubjectAssign = new Domain.Entities.TeacherSubjectAssignment
            {
                TeacherId = teacher.Id,
                SubjectId = subject.Id,
                GradeLevelId = gradeLevel.Id,
                IsActive = true
            };
            await context.TeacherSubjectAssignment.AddAsync(newTeacherSubjectAssign);
            await context.SaveChangesAsync(cancellationToken);
            return new Response<Guid>
            {
                Success = true,
                Message = "Subject assigned successfully",
                Data = newTeacherSubjectAssign.Id
            };
        }
    }
}
