using MediatR;

namespace CenterFlow.Application.Features.Rates.GetTeacherRating
{
    public record GetTeacherRatingQuery(Guid SubjectId, Guid GradeId)
        : IRequest<List<TeacherRatingDto>>;
    public class TeacherRatingDto
    {
        public Guid Id { get; set; }
        public decimal Rating { get; set; }
        public Guid TeacherGradeLevelId { get; set; }
        public string TeacherName { get; set; }
        public int TotalRates { get; set; }
    }
}
