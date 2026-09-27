namespace CenterFlow.Application.Common.Models
{
    public class SendEmailDto
    {
       public string studentName { get; set; }
        public string studentEmail { get; set; }
        public string teacherName { get; set; }      // ← جديد
        public DateOnly date { get; set; }
        public TimeSpan from { get; set; }
        public TimeSpan to { get; set; }
    }
}
