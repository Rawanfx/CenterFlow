namespace CenterFlow.Domain.Entities
{
    public class Teacher:ApplicationUser
    {
        public Subject Subject { get; set; }
        public Guid SubjectId { get; set; }
        public ICollection<TeacherAvailability> TeacherAvailabilities => new List<TeacherAvailability>();
    }
}
