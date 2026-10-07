namespace CenterFlow.Domain.Entities
{
    public class GradeLevel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<TeacherSubjectAssignment> TeacherGradeLevels { get; set; } = new List<TeacherSubjectAssignment>();
    }
}
