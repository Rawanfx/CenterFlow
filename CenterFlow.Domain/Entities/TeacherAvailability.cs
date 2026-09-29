namespace CenterFlow.Domain.Entities
{
    public class TeacherAvailability
    {
        public Guid Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
        public Teacher Teacher { get; set; }
        public Guid TeacherId { get; set; }
        public bool IsDelete { get; set; } = false;
    }
}
