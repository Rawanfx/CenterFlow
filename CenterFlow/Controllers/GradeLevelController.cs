using CenterFlow.Application.Features.GradeLevel.AddGradeLevel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeLevelController : ControllerBase
    {
        private readonly IMediator mediator;
        public GradeLevelController(IMediator mediator) => this.mediator = mediator;
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult>AddGradeLevel (AddGradeLevelCommand command)
        {
          var result=  await mediator.Send(command);
            return Ok(result);
        }
    }
}
