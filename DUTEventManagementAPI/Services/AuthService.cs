using DUTEventManagementAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DUTEventManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly IConfiguration _config;

        public AuthService(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> GenerateTokenString(AppUser user)
        {
            // Await the async call to get the roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create claims, including one claim per role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()!),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            // Add each role as a separate Claim
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<LoginResponse> Login(string email, string password)
        {
            var response = new LoginResponse();
            var userResult = await _userManager.FindByEmailAsync(email);
            if (userResult == null)
            {
                Console.WriteLine("User not found");
                return response;
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(userResult, password);
            if (!passwordCheck)
            {
                Console.WriteLine("Password is incorrect");
                return response;
            }
            response.IsAuthenticated = true;
            response.Token = GenerateTokenString(userResult).Result;
            response.RefreshToken = GenerateRefreshTokenString();

            // Lưu RefreshToken 
            userResult.RefreshToken = response.RefreshToken;
            userResult.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            var result = await _userManager.UpdateAsync(userResult);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                return response;
            }
            return response;

        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenModel model)
        {
            var response = new LoginResponse();
            var principal = GetPrincipalFromExpiredToken(model.Token);

            var email = principal?.Identities.FirstOrDefault()?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine($"Email is not in user claims");
                return response;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                Console.WriteLine($"User not found");
                return response;
            }
            if (user.RefreshToken != model.RefreshToken)
            {
                Console.WriteLine($"Refresh token mismatch");
                return response;
            }
            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                Console.WriteLine($"Refresh token expired");
                return response;
            }

            response.IsAuthenticated = true;
            response.Token = GenerateTokenString(user).Result;
            response.RefreshToken = GenerateRefreshTokenString();

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(2);
            //await userService.UpdateUserPasswordAsync(user.UserId.ToString()!, user.PasswordHashed);

            // Lưu RefreshToken
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                return response;
            }

            return response;
        }

        public async Task<bool> Register(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                return false;
            }

            return await _userManager.AddToRoleAsync(user, "User") == IdentityResult.Success;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // cho phép token hết hạn
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParams, out _);
        }
    }
}
