using System.ComponentModel.DataAnnotations.Schema;

namespace CenterFlow.Domain.Entities
{
    public class TeacherSubjectAssignment
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(Teacher))]
        public string TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        [ForeignKey(nameof(GradeLevel))]
        public Guid GradeLevelId { get; set; }
        public GradeLevel GradeLevel { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Subject Subject { get; set; }
        public Guid SubjectId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
