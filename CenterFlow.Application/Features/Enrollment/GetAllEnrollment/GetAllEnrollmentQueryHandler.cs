using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Enrollment.GetAllEnrollment
{
    public class GetAllEnrollmentQueryHandler : IRequestHandler<GetAllEnrollmentQuery, Response<List<StudentEnrollent>>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public GetAllEnrollmentQueryHandler(IAppDbContext context,ICurrentUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<List<StudentEnrollent>>> Handle(GetAllEnrollmentQuery request, CancellationToken cancellationToken)
        {
            Guid studentId = Guid.Parse(userService.UserId);
            var query = context.StudentBookings
                .AsNoTracking()
                .Where(x => x.StudentId == studentId 
            && x.Status!=Domain.Enum.StudentBookingStatus.Cancelled).AsQueryable();
            if (request.Date.HasValue)
                query = query.Where(x => x.Book.Date == request.Date.Value);
            if (request.SubjectId.HasValue)
                query = query.Where(x => x.Book.TeacherSubjectAssignment.SubjectId == request.SubjectId.Value);
            if (request.TeacherId.HasValue)
                query = query.Where(x => x.Book.TeacherId == request.TeacherId.Value);

            var enrollements =await query
                .OrderBy(x=>x.Book.Date).ThenBy(x=>x.Book.From)
                .Select(x => new StudentEnrollent()
            {
                Date = x.Book.Date,
                EnrollmentId = x.Id,
                From = x.Book.From,
                To= x.Book.To,
                RoomName = x.Book.Room.Name,
                SubjectId = x.Book.TeacherSubjectAssignment.SubjectId,
                TeacherName = x.Book.Teacher.FullName,
                SubjectName = x.Book.TeacherSubjectAssignment.Subject.Name,
                status= x.Status
            }).ToListAsync();
            return new Response<List<StudentEnrollent>>() { Success = true, Data = enrollements };
        }
    }
}
