using Microsoft.AspNetCore.Http;
using DUTEventManagementAPI.Services.Interfaces;
using DUTEventManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;
        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetBadges()
        {
            try
            {
                var badges = await _badgeService.GetBadgesAsync();
                return Ok(badges);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{badgeId}")]
        public async Task<IActionResult> GetBadgeById(string badgeId)
        {
            try
            {
                var badge = await _badgeService.GetBadgeByIdAsync(badgeId);
                if (badge == null)
                {
                    return NotFound();
                }
                return Ok(badge);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateBadge([FromBody] Badge badge)
        {
            if (badge == null)
            {
                return BadRequest("Badge cannot be null.");
            }
            try
            {
                var createdBadge = await _badgeService.CreateBadgeAsync(badge);
                return CreatedAtAction(nameof(GetBadgeById), new { badgeId = createdBadge.BadgeId }, createdBadge);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("{badgeId}")]
        public async Task<IActionResult> UpdateBadge(string badgeId, [FromBody] Badge updatedBadge)
        {
            if (updatedBadge == null)
            {
                return BadRequest("Updated badge cannot be null.");
            }
            try
            {
                var badge = await _badgeService.UpdateBadgeAsync(badgeId, updatedBadge);
                return Ok(badge);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{badgeId}")]
        public async Task<IActionResult> DeleteBadge(string badgeId)
        {
            try
            {
                var result = await _badgeService.DeleteBadgeAsync(badgeId);
                if (result)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Event/{eventId}")]
        public async Task<IActionResult> GetBadgesByEventId(string eventId)
        {
            try
            {
                var badges = await _badgeService.GetBadgesByEventIdAsync(eventId);
                return Ok(badges);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetBadgesByUserId(string userId)
        {
            try
            {
                var badges = await _badgeService.GetBadgesByUserIdAsync(userId);
                return Ok(badges);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
