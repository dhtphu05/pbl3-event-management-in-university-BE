using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services;
using DUTEventManagementAPI.Services.Interfaces;
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

        [HttpGet]
        public IActionResult Get()
        {
            var message = new Message(new string[] { "noreply.itf.dut.event@gmail.com" }, "Test email", "This is the content from our email.");
            _emailService.SendEmail(message);
            return NoContent();
        }
    }
}
