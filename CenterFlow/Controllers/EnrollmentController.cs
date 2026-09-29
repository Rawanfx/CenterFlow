using CenterFlow.Application.Features.Booking.CancelEnrollment;
using CenterFlow.Application.Features.Booking.StudentEnroll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IMediator mediator;
        public EnrollmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpDelete("{enrollId}/cancel")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CancelEnrollment([FromRoute] CancelEnrollmentCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("{bookId}/enroll")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentEnroll([FromRoute] StudentEnrollCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
