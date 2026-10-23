
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Interfaces;
using CenterFlow.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CenterFlow.Application.Features.Booking.GetBooking
{
    public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, Response<List<TeacherBooking>>>
    {
        private readonly IAppDbContext context;
        private readonly ICurrentUserService userService;
        public GetBookingQueryHandler (IAppDbContext context, ICurrentUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        public async Task<Response<List<TeacherBooking>>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var teacher = await context.Teachers
                .FirstOrDefaultAsync(x => x.Id== userService.UserId);
            if (teacher == null)
                throw new NotFoundException("teacher not found");


            var books = context.Books
                 .OrderBy(x => x.Date).ThenBy(x => x.From)
                .Where(x => x.Status != Domain.Enum.BookingStatus.Cancelled
                && x.TeacherId == Guid.Parse(teacher.Id)).AsQueryable();
            if (request.Date.HasValue)
                books = books.Where(x => x.Date == request.Date.Value);

            if (request.SubjectId.HasValue)
                books = books.Where(x => x.TeacherSubjectAssignment.SubjectId == request.SubjectId.Value);
            if (request.GradeLevelId.HasValue)
                books = books.Where(x => x.TeacherSubjectAssignment.GradeLevelId == request.GradeLevelId.Value);
            var result =await books.Select(x => new TeacherBooking()
            { 
              BookId= x.Id,
              Date = x.Date,
              From = x.From,
              NumberOfStudents = x.StudentBookings.Count(x=>x.Status!= Domain.Enum.StudentBookingStatus.Cancelled),
              RoomId = x.RoomId,
              RoomName = x.Room.Name,
              To = x.To,
              Status= x.Status
            }).ToListAsync(cancellationToken);
            return new Response<List<TeacherBooking>>()
            {
                Success = true,
                Data = result
            };
        }
    }
}
