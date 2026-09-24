
namespace CenterFlow.Application.Common.Exceptions
{
    public class InvalidBooking:Exception
    {
        public string message { get; }
        public InvalidBooking (string message):base(message) { this.message = message; }
    }
}
