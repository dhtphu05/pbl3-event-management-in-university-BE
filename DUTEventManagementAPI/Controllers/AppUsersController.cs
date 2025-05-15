using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AppUsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        public AppUsersController(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var user = _context.AppUsers.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public IActionResult GetUserById(string userId)
        {
            var user = _context.AppUsers.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateUser(string userId, [FromBody] AppUser updatedUser)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }
            var user = _context.AppUsers.Find(userId);
            user = _userService.Update(userId, updatedUser);
            return Ok();
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();
            var user = _context.AppUsers.Find(userId);
            _userService.Delete(userId);
            return Ok();
        }
    }
}
