using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Services;
using DUTEventManagementAPI.Models;

using DUTEventManagementAPI.Data;

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
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest("Event data is null");
            }
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
    }
}
