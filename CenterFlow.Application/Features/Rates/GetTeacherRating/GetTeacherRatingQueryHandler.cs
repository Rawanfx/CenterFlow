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
                && x.SubjectId == request.SubjectId)
                .Select(x =>new { x.Id ,
                x.Teacher.FullName
                })
                .ToListAsync(cancellationToken);
            if (!teacherSubjectAssignments.Any())
                return new List<TeacherRatingDto>();
            var teacherSubjectAssignmentIds = teacherSubjectAssignments.Select(x => x.Id).ToList();
            var ratings =await context.Rates
                .Where(x=> teacherSubjectAssignmentIds.Contains(x.TeacherGradeLevelId))
                .GroupBy(x=>x.TeacherGradeLevelId)
                .Select(x=> new 
                {
                    TeacherGradeLevelId = x.Key,
                    Rating = x.Average(r => r.Rate),
                    RateCount = x.Count(),
                })
                .ToListAsync(cancellationToken);

            var result = teacherSubjectAssignments.Select(x => {
                var rating = ratings.FirstOrDefault(y => y.TeacherGradeLevelId == x.Id);
                return new TeacherRatingDto()
                {
                    Rating = rating?.Rating ?? 0,
                    TeacherGradeLevelId = x.Id,
                    TeacherName = x.FullName,
                    TotalRates = rating?.RateCount ?? 0

                };
            })
                .OrderByDescending(x=>x.Rating)
                .ToList();
            return result;
        }
    }
}
