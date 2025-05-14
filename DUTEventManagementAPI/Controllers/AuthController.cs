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

            return BadRequest(new {message = "Register failed"});
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


            var properties = _authService.ConfigureGoogleAuthenticationProperties(
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
    }
}
