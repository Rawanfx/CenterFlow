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
        await emailService.SendBookingCancelledEmailAsync(
            "Rawan",
            "rawanseyed580@gmail.com",
            "Ahmed", DateOnly.MinValue, new TimeSpan(15, 0, 0), new TimeSpan(16, 0, 0));
           
          
        return Ok("Sent");
    }
}