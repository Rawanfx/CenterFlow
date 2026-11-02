using CenterFlow.Domain.Enum;

namespace CenterFlow.Domain.Entities
{
    public class RecurringBooking
    {
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
        public Guid TeacherSubjectAssignId { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public RecurringType Type { get; set; }
        public DateOnly StartDate { get; set; }
        public int Count { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Book> Books { get; set; } = new();
    }
}
