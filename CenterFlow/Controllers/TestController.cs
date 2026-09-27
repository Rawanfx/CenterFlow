using CenterFlow.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IEmailService emailService;
    public TestController(IEmailService emailService) => this.emailService = emailService;

    [HttpGet("send-test-email")]
    public async Task<IActionResult> SendTest()
    {
           
          
        return Ok("Sent");
    }
}