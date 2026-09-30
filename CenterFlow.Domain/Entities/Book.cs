using CenterFlow.Domain.Common;
using CenterFlow.Domain.Enum;
using CenterFlow.Domain.Events;

namespace CenterFlow.Domain.Entities
{
    public class Book:BaseEntity
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
        public List<StudentBooking> StudentBookings = new List<StudentBooking>();
        public static Book Create(Guid teacherId,
     Guid roomId,
     DateOnly date,
     TimeSpan from,
     TimeSpan to)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                TeacherId = teacherId,
                RoomId = roomId,
                Date = date,
                From = from,
                To = to,
                Status = BookingStatus.Confirmed
            };
            book.RaiseDomainEvent(new BookingCreatedDomainEvent(
    book.Id,
    teacherId.ToString(),
    string.Empty,
    date,
    from,
    to));
            return book;
        }


    }
 
}