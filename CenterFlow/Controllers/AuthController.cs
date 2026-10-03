using CenterFlow.Application.Features.Auth.Login;
using CenterFlow.Application.Features.Auth.StudentRegister;
using CenterFlow.Application.Features.Auth.TeacherRegister;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CenterFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;
        public AuthController(IMediator mediator) => this.mediator = mediator;
        [HttpPost("teacher-register")]
        public async Task<IActionResult>TeacherRegister(TeacherRegisterCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("student-register")]
        public async Task<IActionResult> StudentRegister(StudentRegisterCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
    }
}
