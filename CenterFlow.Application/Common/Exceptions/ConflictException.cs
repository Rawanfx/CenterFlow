
namespace CenterFlow.Application.Common.Exceptions
{
    public class ConflictException:AppException
    {
        public SkipReason SkipReason { get;  }
        public ConflictException (string message, SkipReason skipReason):base(message,409) {
           this. SkipReason = skipReason;
        }
    }
}
