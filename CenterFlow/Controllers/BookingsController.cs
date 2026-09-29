using CenterFlow.Application.Features.Booking.CancelBooking;
using CenterFlow.Application.Features.Booking.CancelEnrollment;
using CenterFlow.Application.Features.Booking.CreateBooking;
using CenterFlow.Application.Features.Booking.StudentEnroll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        
    }
}
