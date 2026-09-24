
namespace CenterFlow.Domain.Entities
{
    public class Room
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public string Name { get; set; }
        public Guid CenterId { get; set; }
        public Center Center { get; set; }
    }
}
