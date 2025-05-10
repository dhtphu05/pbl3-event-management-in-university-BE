using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Services;
using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;
        public TimeSlotsController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }
        [HttpGet]
        public IActionResult GetAllTimeSlots()
        {
            var timeSlots = _timeSlotService.GetAllTimeSlots();
            if (timeSlots == null || timeSlots.Count == 0)
            {
                return NotFound("No time slots found");
            }
            return Ok(timeSlots);
        }
        [HttpGet("{ISOtimeString}")]
        public IActionResult GetTimeSlots(string ISOtimeString)
        {
            var time = DateTime.Parse(ISOtimeString);
            var timeSlot = _timeSlotService.GetTimeSlotByTime(time);
            if (timeSlot == null)
            {
                return NotFound("No time slot found for the given time");
            }
            return Ok(timeSlot);
        }
        [HttpGet("Event/{eventId}")]
        public IActionResult GetTimeSlotsByEventId(string eventId)
        {
            var timeSlots = _timeSlotService.GetTimeSlotsByEventId(eventId);
            if (timeSlots == null || timeSlots.Count == 0)
            {
                return NotFound("No time slots found for the given event");
            }
            return Ok(timeSlots);
        }
        [HttpPost]
        public IActionResult AddTimeSlot([FromBody] TimeSlot timeSlot)
        {
            if (timeSlot == null)
            {
                return BadRequest("Time slot data is null");
            }
            try
            {
                var createdTimeSlot = _timeSlotService.AddTimeSlot(timeSlot);
                return CreatedAtAction(nameof(GetTimeSlots), new { ISOtimeString = createdTimeSlot.StartTime.ToString("o") }, createdTimeSlot);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public IActionResult RemoveTimeSlot(string id)
        {
            var result = _timeSlotService.RemoveTimeSlot(id);
            if (!result)
            {
                return NotFound("Time slot not found");
            }
            return NoContent();
        }
    }
}
