namespace CenterFlow.Domain.Entities
{
    public class TeacherRatings
    {
        public Guid Id { get; set; }
        public string StudentId { get; set; }
        public Student Student { get; set; }
        public decimal Rate { get; set; }
        public string? Comment { get; set; }
        public Guid TeacherGradeLevelId { get; set; }
        public TeacherSubjectAssignment TeacherGradeLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public StudentBooking StudentBooking { get; set; }
        public Guid StudentBookingId { get; set; }
    }

}
