namespace CenterFlow.Application.Common.Exceptions
{
    public class IdentityException:Exception
    {
        public List<string> Errors { get;}
        public IdentityException(List<string> errors):base()
        {
            Errors = errors;
        }
    }
}
