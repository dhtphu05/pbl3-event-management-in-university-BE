using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services;
using DUTEventManagementAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IEventService _eventService;
        public EmailController(IEmailService emailService, IEventService eventService)
        {
            _emailService = emailService;
            _eventService = eventService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var message = new Message(new string[] { "lostpeanut123@gmail.com" }, "Test email", "This is the content from our email.");
            _emailService.SendEmail(message);
            return NoContent();
        }

        [HttpPost("SendRegisteredConfirmation/{eventId}")]
        public IActionResult SendRegisteredConfirmationEmail(string[] recipientEmails, string eventId)
        {
            try
            {
                // Kiểm tra input
                if (recipientEmails == null || !recipientEmails.Any())
                {
                    return BadRequest("Recipient emails list is required");
                }

                // Tìm sự kiện theo ID
                var eventModel = _eventService.GetEventByIdAsync(eventId);
                if (eventModel.Result == null)
                {
                    return NotFound("Event not found");
                }

                // Gửi email xác nhận đăng ký
                string templatePath = "Views/EmailTemplates/RegistrationConfirmation.cshtml";
                string subject = $"Xác nhận đăng ký sự kiện: {eventModel.Result.EventName}";

                 _emailService.SendEmailWithTemplateAsync(
                    eventModel.Result,
                    recipientEmails,
                    templatePath,
                    subject);

                return Ok(new { message = "Email confirmation sent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
