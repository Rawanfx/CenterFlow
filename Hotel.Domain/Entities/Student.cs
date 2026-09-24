
using CenterFlow.Domain.Enum;

namespace CenterFlow.Domain.Entities
{
    public class Student:ApplicationUser
    {
        public GradeLevel GradeLevel { get; set; }
    }
}
