using CenterFlow.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Rates.GetTeacherRating
{
    public class GetTeacherRatingQueryHandler : IRequestHandler<GetTeacherRatingQuery, List<TeacherRatingDto>>
    {
        private readonly IAppDbContext context;
        public GetTeacherRatingQueryHandler(IAppDbContext context)
        {
            this.context = context;
        }
        public async Task<List<TeacherRatingDto>> Handle(GetTeacherRatingQuery request, CancellationToken cancellationToken)
        {
            var teacherSubjectAssignments = await context.TeacherSubjectAssignment
                .Where(x => x.GradeLevelId == request.GradeId
                && x.SubjectId == request.StudentId)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
