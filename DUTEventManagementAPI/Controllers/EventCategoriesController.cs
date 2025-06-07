using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventCategoriesController : ControllerBase
    {
        private readonly IEventCategoryService _eventCategoryService;
        public EventCategoriesController(IEventCategoryService eventCategoryService)
        {
            _eventCategoryService = eventCategoryService;
        }
        [HttpGet]
        public IActionResult GetAllEventCategories()
        {
            var eventCategories = _eventCategoryService.GetAllEventCategories();
            if (eventCategories == null || eventCategories.Count == 0)
            {
                return NotFound("No event categories found");
            }
            return Ok(eventCategories);
        }
        [HttpPost("{eventId}/Categories")]
        public IActionResult AssignCategoryToEvent(string eventId, [FromBody] List<string> categoriesName)
        {
            if (string.IsNullOrEmpty(eventId) || categoriesName == null || categoriesName.Count == 0)
            {
                return BadRequest("Event ID or Category Names are null");
            }
            try
            {
                foreach (var categoryName in categoriesName)
                {
                    if (string.IsNullOrEmpty(categoryName))
                    {
                        return BadRequest("Category Name is null or empty");
                    }
                    _eventCategoryService.CreateEventCategory(eventId, categoryName);
                }
                return Ok("Categories assigned to event successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{categoryName}/Events")]
        public IActionResult GetEventsByCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Category Name is null");
            }
            try
            {
                var events = _eventCategoryService.GetEventsByCategory(categoryName);
                if (events == null || events.Count == 0)
                {
                    return NotFound("No events found for this category");
                }
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{eventId}/Categories")]
        public IActionResult GetCategoriesOfEvent(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest("Event ID is null");
            }
            try
            {
                var categories = _eventCategoryService.GetCategoriesOfEvent(eventId);
                if (categories == null || categories.Count == 0)
                {
                    return NotFound("No categories found for this event");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{eventId}/{categoryName}")]
        public IActionResult DeleteEventCategory(string eventId, string categoryName)
        {
            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(categoryName))
            {
                return BadRequest("Event ID or Category Name is null");
            }
            try
            {
                var result = _eventCategoryService.DeleteEventCategory(eventId, categoryName);
                if (!result)
                {
                    return NotFound("Event or Category not found");
                }
                return Ok("Event category deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
