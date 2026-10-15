using CenterFlow.Application.Common.Models;
using MediatR;
namespace CenterFlow.Application.Features.Rates.GetAllRatings
{
    public record GetAllRatingsQuery(int pageSize = 1,
       int PageNumber =10,
       decimal? MinRate = null,
       decimal?maxRate=null,
       DateTime ? From =null,
       DateTime ? To =null,
       string ?TeacherId=null,
       Guid ? SubjectId = null,
       Guid ? GradeLevelId = null
        ):IRequest<PageResult<List<AdminRatingResult>>>;
    public class AdminRatingResult
    {

        public Guid Id { get; set; }
        public decimal Rate { get; set; }
        public string? Comment { get; set; }

        public string StudentName { get; set; } = default!;
        public string TeacherName { get; set; } = default!;
        public string SubjectName { get; set; } = default!;
        public string GradeLevelName { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
    }
}
