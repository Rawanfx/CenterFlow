namespace CenterFlow.Domain.Entities
{
    public class Teacher:ApplicationUser
    {
        public Subject Subject { get; set; }
        public Guid SubjectId { get; set; }
        public double? ExperienceYear { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
        public 
        public List<TeacherSubjectAssignment> TeacherGradeLevels { get; set; } = new List<TeacherSubjectAssignment>();
        public ICollection<TeacherAvailability> TeacherAvailabilities => new List<TeacherAvailability>();
    }
}
