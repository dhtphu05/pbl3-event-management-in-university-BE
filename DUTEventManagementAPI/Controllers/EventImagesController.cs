using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventImagesController : ControllerBase
    {
        private readonly IEventImageService _eventImageService;
        public EventImagesController(IEventImageService eventImageService)
        {
            _eventImageService = eventImageService;
        }
        [HttpGet]
        public IActionResult GetAllEventImages()
        {
            var eventImages = _eventImageService.GetAllEventImages();
            if (eventImages == null)
            {
                return NotFound("Event images not found");
            }
            return Ok(eventImages);
        }
        [HttpGet("{eventId}")]
        public IActionResult GetAllImagesOfAnEvent(string eventId)
        {
            var eventImages = _eventImageService.GetAllImagesOfAnEvent(eventId);
            if (eventImages == null)
            {
                return NotFound("Event images not found");
            }
            return Ok(eventImages);
        }
        [HttpPost]
        public IActionResult AddEventImage([FromBody] EventImage eventImage)
        {
            if (eventImage == null)
            {
                return BadRequest("Event image is null");
            }
            var result = _eventImageService.AddEventImage(eventImage);
            if (!result)
            {
                return BadRequest("Failed to add event image");
            }
            return CreatedAtAction(nameof(GetAllImagesOfAnEvent), new { eventId = eventImage.EventId }, eventImage);
        }
        [HttpDelete("{eventImageId}")]
        public IActionResult DeleteEventImage(string eventImageId)
        {
            var result = _eventImageService.DeleteEventImage(eventImageId);
            if (!result)
            {
                return NotFound("Event image not found");
            }
            return NoContent();
        }
    }
}
