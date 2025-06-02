using DUTEventManagementAPI.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IAuthService
    {
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUri);
        string GenerateRefreshTokenString();
        Task<string> GenerateTokenString(AppUser user);
        Task<LoginResponse> Login(string email, string password);
        Task<LoginResponse> LoginWithGoogle(ClaimsPrincipal? claimsPrincipal);
        Task<LoginResponse> RefreshToken(RefreshTokenModel model);
        Task<bool> Register(AppUser user, string password);
        List<string> GetUserRoles(string userId);
        Task<bool> AddUserToRole(string userId, string roleName);
        Task<bool> RemoveUserFromRole(string userId, string roleName);
    }
}