using CenterFlow.Application.Features.Availability.AddAvailableSessionsForTeacher;
using CenterFlow.Application.Features.Availability.DeleteAvailableSlot;
using CenterFlow.Application.Features.Availability.GetAvailableSlots;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IMediator mediator;
        public AvailabilityController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> AddAvailabilableSession([FromBody]AddAvailableSessionCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpDelete("{id:guid}")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> Delete(
       Guid id,
       CancellationToken cancellationToken)
        {
            await mediator.Send(new DeleteAvailableSlotCommand(id), cancellationToken);
            return NoContent();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAvailableSessions([FromBody] GetAvailableSessionsQuery query)
        {
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
