namespace CenterFlow.Application.Common.Exceptions
{
    public class AppException:Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        protected AppException(string message, int statusCode, Exception inner)
            : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }
}
