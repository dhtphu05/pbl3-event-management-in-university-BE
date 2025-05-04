using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using DUTEventManagementAPI.Data;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppUsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AppUsersController(AppDbContext context)
        {
            _context = context;
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

    }
}
