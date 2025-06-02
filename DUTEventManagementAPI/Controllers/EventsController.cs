using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Models;

using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly AppDbContext _context;
        public EventsController(IEventService eventService, AppDbContext context)
        {
            _eventService = eventService;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllEvents()
        {
            var events = _eventService.GetAllEvents();
            if (events == null || events.Count == 0)
            {
                return NotFound("No events found");
            }
            return Ok(events);
        }
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventById(string eventId)
        {
            var eventDetails = await _eventService.GetEventByIdAsync(eventId);
            if (eventDetails == null)
            {
                return NotFound();
            }
            return Ok(eventDetails);
        }
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest("Event data is null");
            }
            newEvent.HostId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var result = await _eventService.CreateEventAsync(newEvent);
            if (!result.Succeeded)
            {
                return BadRequest("Event creation failed");
            }
            return CreatedAtAction(nameof(GetEventById), new { eventId = newEvent.EventId }, newEvent);
        }
        [HttpPut("{eventId}")]
        public IActionResult UpdateEventAsync(string eventId, [FromBody] Event updatedEvent)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest();
            }
            var eventToUpdate = _context.Events.Find(eventId);
            if (eventToUpdate == null)
            {
                return NotFound();
            }
            _eventService.UpdateEventAsync(eventId, updatedEvent);
            return Ok();
        }
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(string eventId)
        {
            var result = await _eventService.DeleteEventAsync(eventId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
        [HttpPost("{eventId}/OpenForRegistration")]
        public IActionResult OpenEventForRegistration(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest("Event ID is required");
            }
            try
            {
                var openResult = _eventService.OpenEventForRegistration(eventId);
                if (!openResult.Result)
                {
                    return BadRequest("Failed to open event for registration");
                }
                return Ok("Event opened for registration successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error opening event for registration: {ex.Message}");
            }
        }
        [HttpPost("{eventId}/CloseForRegistration")]
        public IActionResult CloseEventForRegistration(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest("Event ID is required");
            }
            try
            {
                var closeResult = _eventService.CloseEventForRegistration(eventId);
                if (!closeResult.Result)
                {
                    return BadRequest("Failed to close event for registration");
                }
                return Ok("Event closed for registration successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error closing event for registration: {ex.Message}");
            }
        }
        [HttpPost("{eventId}/AddFacultiesToScope")]
        public IActionResult AddFacultiesToScope(string eventId, List<string> facultyIds)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest("Event ID is required");
            }
            try
            {
                foreach (var facultyId in facultyIds)
                {
                    var result = _eventService.AddFacultyToScope(eventId, facultyId);
                    if (!result.Result)
                    {
                        return BadRequest($"Failed to add faculty with ID {facultyId} to event scope");
                    }
                }
                return Ok("Faculties added to event scope successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding faculties: {ex.Message}");
            }
        }
        [HttpGet("{eventId}/GetFacultiesInScope")]
        public IActionResult GetFacultiesInScope(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest("Event ID is required");
            }
            try
            {
                var faculties = _eventService.GetFacultiesInScope(eventId);
                if (faculties == null || !faculties.Any())
                {
                    return NotFound("No faculties found in event scope");
                }
                return Ok(faculties);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving faculties: {ex.Message}");
            }
        }
        [HttpPost("{eventId}/RemoveFacultiesFromScope")]
        public IActionResult RemoveFacultiesFromScope(string eventId, List<string> facultyIds)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest("Event ID is required");
            }
            try
            {
                foreach (var facultyId in facultyIds)
                {
                    var result = _eventService.RemoveFacultyFromScope(eventId, facultyId);
                    if (!result.Result)
                    {
                        return BadRequest($"Failed to remove faculty with ID {facultyId} from event scope");
                    }
                }
                return Ok("Faculties removed from event scope successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error removing faculties: {ex.Message}");
            }
        }
    }
}
