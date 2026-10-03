namespace CenterFlow.Application.Common.Exceptions
{
    public class IdentityException:AppException
    {
        public IdentityException(List<string> errors)
            :base(string.Join(',',errors),400)
        {
        }
    }
}
