
using CenterFlow.Application.Common.Exceptions;
using CenterFlow.Application.Common.Models;

namespace CenterFlow
{
    public class GlobalExceptionHandling : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                //  Log.Error($"Exception Thrown {ex.Message} ");
                await HandleException(context, ex);
            }
        }
        private async Task HandleException(HttpContext context, Exception ex)
        {
            var (response, statusCode) = ex switch
            {
                IdentityException exception => (
                    new Response<object> { Success = false, Errors = exception.Errors },
                    StatusCodes.Status400BadRequest
                ),

                NotFoundException exception => (
                    new Response<object> { Success = false, Message = exception.Message },
                    StatusCodes.Status404NotFound
                ),

                InvalidBooking exception=> (new Response<object>
                {
                    Success = false,
                    Message = exception.Message
                },StatusCodes.Status400BadRequest ),

                _ => (
                    new Response<object> { Success = false, Message = "An error occurred" },
                    StatusCodes.Status500InternalServerError
                )
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}