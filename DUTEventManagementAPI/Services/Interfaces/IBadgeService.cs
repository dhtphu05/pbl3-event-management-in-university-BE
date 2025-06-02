using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IBadgeService
    {
        Task<UserBadge> AssignBadgeToUserAsync(string userId, string badgeId);
        Task<Badge> CreateBadgeAsync(Badge badge);
        Task<bool> DeleteBadgeAsync(string badgeId);
        Task<Badge?> GetBadgeByIdAsync(string badgeId);
        Task<List<Badge>> GetBadgesAsync();
        Task<List<Badge>> GetBadgesByEventIdAsync(string eventId);
        Task<List<Badge>> GetBadgesByUserIdAsync(string userId);
        Task<bool> RemoveBadgeFromUserAsync(string userId, string badgeId);
        Task<Badge> UpdateBadgeAsync(string badgeId, Badge updatedBadge);
    }
}