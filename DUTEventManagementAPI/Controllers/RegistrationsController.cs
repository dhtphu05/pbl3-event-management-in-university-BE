using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        public RegistrationsController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }
        [HttpGet]
        public IActionResult GetAllRegistrations()
        {
            var registrations = _registrationService.GetAllRegistrations();
            if (registrations == null || registrations.Count == 0)
            {
                return NotFound("No registrations found");
            }
            return Ok(registrations);
        }
        [HttpGet("Users/{eventId}")]
        public IActionResult GetUsersRegisteredForEvent(string eventId)
        {
            var users = _registrationService.GetUsersRegisteredForEvent(eventId);
            if (users == null || users.Count == 0)
            {
                return NotFound("No users registered for this event");
            }
            return Ok(users);
        }
        [HttpGet("Events/{userId}")]
        public IActionResult GetEventsUserRegisteredFor(string userId)
        {
            var events = _registrationService.GetEventsUserRegisteredFor(userId);
            if (events == null || events.Count == 0)
            {
                return NotFound("No events registered for this user");
            }
            return Ok(events);
        }
        [HttpPost("{userId}/{eventId}")]
        public IActionResult RegisterUserForEvent(string userId, string eventId)
        {
            try
            {
                var registration = _registrationService.RegisterUserForEvent(userId, eventId);
                return CreatedAtAction(nameof(GetAllRegistrations), new { userId, eventId }, registration);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpDelete("{eventId}/{userId}")]
        public IActionResult RemoveRegistration(string eventId, string userId)
        {
            var result = _registrationService.RemoveRegistration(eventId, userId);
            if (!result)
            {
                return NotFound("Registration not found");
            }
            return NoContent();
        }
    }
}
