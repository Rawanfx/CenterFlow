using CenterFlow.Application.Features.TeacherSubjectAssign.AddTeacherSubjectAssign;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly IMediator mediator;
        [HttpPost("assign-subject")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult>TeacherAssign (AddTeacherSubjectAssignCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
