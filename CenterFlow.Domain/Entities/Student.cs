
using CenterFlow.Domain.Enum;

namespace CenterFlow.Domain.Entities
{
    public class Student:ApplicationUser
    {
        public Guid GradeLevelId { get; set; }
        public GradeLevel GradeLevel { get; set; }
    }
}
