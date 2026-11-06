
namespace CenterFlow.Application.Common.Models.OutboxMessage
{
    public class BookingReminderEvent
    {
        public Guid BookId { get; set; }
        public DateTime RunAt { get; set; }
    }
}
