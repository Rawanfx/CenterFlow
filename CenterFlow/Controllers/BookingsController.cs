using CenterFlow.Application.Features.Booking.CancelBooking;
using CenterFlow.Application.Features.Booking.CancelEnrollment;
using CenterFlow.Application.Features.Booking.CreateBooking;
using CenterFlow.Application.Features.Booking.GetBooking;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator mediator;
        public BookingsController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost]
        [Authorize(Roles = "Teacher")]

        public async Task<IActionResult> Book(CreateBookingCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        
        [HttpDelete("{bookId}/cancel")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> CandelBook([FromRoute] CancelBookingCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetBooking([FromRoute] GetBookingQuery query)
        {
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
