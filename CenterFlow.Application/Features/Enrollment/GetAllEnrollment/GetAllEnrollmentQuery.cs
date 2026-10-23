using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Enum;
using MediatR;

namespace CenterFlow.Application.Features.Enrollment.GetAllEnrollment
{
    public record GetAllEnrollmentQuery(Guid ?SubjectId,Guid? TeacherId,DateOnly?Date
        ):IRequest<Response<List<StudentEnrollent>>>
    {
    }
    public class StudentEnrollent
    {
        public Guid EnrollmentId { get; set; }
        public string TeacherName { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan  From { get; set; }
        public TimeSpan  To { get; set; }
        public string RoomName { get; set; }
        public StudentBookingStatus status { get; set; }
    }
}
