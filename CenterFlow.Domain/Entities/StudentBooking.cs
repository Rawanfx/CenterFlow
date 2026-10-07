using CenterFlow.Domain.Enum;

namespace CenterFlow.Domain.Entities
{
    public class StudentBooking
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime EnrolledAt { get; set; }
        public StudentBookingStatus Status { get; set; } = StudentBookingStatus.Pending;
        public DateTime? HoldExpiresAt { get; set; }
    }
}
