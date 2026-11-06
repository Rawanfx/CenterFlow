
namespace CenterFlow.Application.Common.Models.OutboxMessage
{
    public class BookingNotificationEvent
    {
        public Guid BookingId { get; set; }

        public string UserId { get; set; } = null!;

        public string Title { get; set; } = null!;
                                                            
        public string Body { get; set; } = null!;

        public NotificationType Type { get; set; }

        public string ReferenceId { get; set; } = null!;
    }
}
