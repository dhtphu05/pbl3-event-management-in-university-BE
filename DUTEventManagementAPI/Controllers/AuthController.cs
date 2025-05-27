using DUTEventManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DUTEventManagementAPI.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using DUTEventManagementAPI.Services.Interfaces;


namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
            };

            var result = await _authService.Register(user, model.Password);
            if (result)
                return Ok(new { message = "User registered successfully" });

            return BadRequest(new { message = "Register failed" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _authService.Login(model.Email!, model.Password!);
            if (result.IsAuthenticated)
                return Ok(result);

            return Unauthorized(new { message = "Login failed" });

        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
        {
            var loginResult = await _authService.RefreshToken(model);
            if (loginResult.IsAuthenticated)
            {
                return Ok(loginResult);
            }

            return Unauthorized();
        }

        [HttpGet("Login/Google")]
        public IActionResult GoogleLogin([FromQuery] string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest(new { message = "Return URL is required" });
            }
            LinkGenerator linkGenerator = HttpContext.RequestServices.GetRequiredService<LinkGenerator>();


            var properties = _authService.ConfigureExternalAuthenticationProperties(
                "Google",
                linkGenerator.GetPathByName(HttpContext, "GoogleLoginCallback") + $"?returnUrl={returnUrl}"
            );

            return Challenge(properties, ["Google"]);
        }

        [HttpGet("Login/Google/Callback", Name = "GoogleLoginCallback")]
        public async Task<IActionResult> GoogleLoginCallBack([FromQuery] string returnUrl)
        {
            Console.WriteLine("GoogleLoginCallBack invoked");
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized(new { message = "External login failed" });

            var authResult = await _authService.LoginWithGoogle(result.Principal);

            if (!authResult.IsAuthenticated)
                return Unauthorized(new { message = "External login failed" });

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return BadRequest(new { authResult, message = "No return url" });
        }

        [HttpGet("Roles/{userId}")]
        public IActionResult GetUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required");
            }
            try
            {
                var roles = _authService.GetUserRoles(userId);
                if (roles == null || roles.Count == 0)
                {
                    return NotFound("No roles found for this user");
                }
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error retrieving user: {ex.Message}" });
            }      
        }
        [HttpPost("Roles/{userId}/Add")]
        public async Task<IActionResult> AddUserToRole(string userId, [FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                return BadRequest("User ID and role name are required");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _authService.AddUserToRole(userId, roleName);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to add user to role" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error retrieving user: {ex.Message}" });
            }
            return Ok(new { message = "User added to role successfully" });
        }
        [HttpPost("Roles/{userId}/Remove")]
        public async Task<IActionResult> RemoveUserFromRole(string userId, [FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(roleName))
            {
                return BadRequest("User ID and role name are required");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _authService.RemoveUserFromRole(userId, roleName);
                if (!result)
                {
                    return BadRequest(new { message = "Failed to remove user from role" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error retrieving user: {ex.Message}" });
            }
            return Ok(new { message = "User removed from role successfully" });
        }
    }
}
