using CenterFlow.Domain.Enum;

namespace CenterFlow.Domain.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public Guid TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public Guid RoomId { get; set; }
        public Room Room { get; set; }
        public DateTime  Date { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public int StudentCount { get; set; }
        public BookingStatus Status { get; set; }
     
    }
}