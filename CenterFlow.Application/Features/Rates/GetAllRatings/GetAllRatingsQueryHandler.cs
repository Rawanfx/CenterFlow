using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace CenterFlow.Application.Features.Rates.GetAllRatings
{
    public class GetAllRatingsQueryHandler : IRequestHandler<GetAllRatingsQuery, PageResult<List<AdminRatingResult>>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public GetAllRatingsQueryHandler(IAppDbContext context,ICurrentUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<PageResult<List<AdminRatingResult>>> Handle(GetAllRatingsQuery request, CancellationToken cancellationToken)
        {
            var user =await context.Admins.FirstOrDefaultAsync(x => x.Id == userService.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            var query = context.Rates.AsNoTracking().AsQueryable();
            if (request.TeacherId != null)
            {
              query= query.Where(x => x.TeacherGradeLevel.TeacherId == request.TeacherId);
            }
            if (request.MinRate.HasValue)
            {
                decimal minRate = request.MinRate.Value;
                query = query.Where(x => x.Rate >= minRate);
            }
            if (request.maxRate.HasValue)
            {
                decimal maxRate = request.maxRate.Value;
                query = query.Where(x => x.Rate <= maxRate);
            }
            if (request.From.HasValue)
            {
                query = query.Where(x => (x.CreatedAt) >= request.From.Value);
            }
            if (request.To.HasValue) 
                query = query.Where(x =>(x.CreatedAt) <= request.To.Value);
            if (request.SubjectId.HasValue)
                query = query.Where(x => x.TeacherGradeLevel.SubjectId == request.SubjectId.Value);
            if (request.GradeLevelId.HasValue)
                query = query.Where(x => x.TeacherGradeLevel.GradeLevelId == request.GradeLevelId.Value);
            var totalCount =await query.CountAsync(cancellationToken);
            var page = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.pageSize < 1 ? 10 :( request.pageSize>100?100:request.pageSize);
            List<AdminRatingResult> queryResult = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((-request.PageNumber + 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AdminRatingResult()
                {
                  CreatedAt=x.CreatedAt,
                GradeLevelName = x.TeacherGradeLevel.GradeLevel.Name,
               Id = x.Id,
               Rate =x.Rate,
               StudentName = x.Student.FullName,
               SubjectName =x.TeacherGradeLevel.Subject.Name,
               TeacherName = x.TeacherGradeLevel.Teacher.FullName,
               Comment=x.Comment     
                }).ToListAsync();
            return new PageResult<List<AdminRatingResult>>() { Data = queryResult, TotalCount = totalCount, Page = request.PageNumber, PageSize = pageSize };
        }
    }
}
