namespace CenterFlow.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = default!;   // المستخدم اللي هيستقبل الإشعار
        public string Title { get; set; } = default!;
        public string Body { get; set; } = default!;
        public NotificationType Type { get; set; }
        public string? ReferenceId { get; set; }         // مثلاً BookingId
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
