using CenterFlow.Application.Features.Booking.CreateBooking;
using MediatR;
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
        public async Task<IActionResult>Book(CreateBookingCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
