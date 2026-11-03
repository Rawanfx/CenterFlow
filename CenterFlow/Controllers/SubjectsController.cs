using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly IMediator mediator;
        public SubjectsController(IMediator mediator) => this.mediator = mediator;
        [HttpPost]
        public async Task<IActionResult> AddSubject(AddSubjectCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
