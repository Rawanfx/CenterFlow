using CenterFlow.Application.Features.Availability.GetAvailableSlots;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly IMediator mediator;
        public SessionsController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAvailableSessions([FromBody]GetAvailableSessionsQuery query)
        {
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}
