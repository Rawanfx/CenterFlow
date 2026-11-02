using CenterFlow.Application.Common.Models;
using CenterFlow.Domain.Enum;
using MediatR;

namespace CenterFlow.Application.Features.Booking.GetBooking;

public record GetBookingQuery(Guid? SubjectId, Guid? GradeLevelId, DateOnly ? Date) : IRequest<Response<List<TeacherBooking>>>;
public class TeacherBooking
{
    public Guid BookId { get; set; }
    public Guid RoomId { get; set; }
    public string RoomName { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
    public int NumberOfStudents { get; set; }
    public BookingStatus Status { get; set; }
}
