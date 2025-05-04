using Microsoft.AspNetCore.Identity;

namespace DUTEventManagementAPI.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        string FacultyId { get; set; } = string.Empty;
        string UserImageUrl { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
