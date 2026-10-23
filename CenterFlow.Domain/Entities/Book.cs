using CenterFlow.Domain.Common;
using CenterFlow.Domain.Enum;
using CenterFlow.Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace CenterFlow.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
        public DateOnly  Date { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public BookingStatus Status { get; set; }
        public List<StudentBooking> StudentBookings { get; set; } = new List<StudentBooking>();
        [ForeignKey(nameof (TeacherSubjectAssignment))]
        public Guid TeacherSubjectAssignId { get; set; }
        public TeacherSubjectAssignment TeacherSubjectAssignment { get; set; }
        public DateTime? ReminderSentAt { get; set; }


    }
 
}