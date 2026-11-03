namespace CenterFlow.Domain.Entities
{
    public class GradeLevel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public List<TeacherSubjectAssignment> TeacherGradeLevels { get; set; } = new List<TeacherSubjectAssignment>();
    }
}
