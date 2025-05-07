using DUTEventManagementAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService
                ?? throw new ArgumentNullException(nameof(emailService));
        }

        [HttpGet("SingleMail")]
        public async Task<IActionResult> SendSingleEmail()
        {
            EmailMetadata emailMetadata = new("john.doe@gmail.com",
                "FluentEmail Test email",
                "This is a test email from FluentEmail.");

            await _emailService.Send(emailMetadata);

            return Ok();
        }
    }
}
