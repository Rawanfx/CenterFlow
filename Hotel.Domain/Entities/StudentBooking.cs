using System;
using System.Collections.Generic;
using System.Text;

namespace CenterFlow.Domain.Entities
{
    public class StudentBooking
    {
        
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
