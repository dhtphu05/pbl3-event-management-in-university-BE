using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace DUTEventManagementAPI.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly AppDbContext _context;
        public BadgeService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Badge>> GetBadgesAsync()
        {
            var badges = await _context.Badges.ToListAsync();
            if (badges == null || !badges.Any())
            {
                throw new Exception("No badges found.");
            }
            return badges;
        }
        public async Task<Badge?> GetBadgeByIdAsync(string badgeId)
        {
            return await _context.Badges.FindAsync(badgeId);
        }
        public async Task<Badge> CreateBadgeAsync(Badge badge)
        {
            _context.Badges.Add(badge);
            if (badge.EventId != null)
            {
                var eventExists = await _context.Events.AnyAsync(e => e.EventId == badge.EventId);
                if (!eventExists)
                {
                    throw new Exception("Event associated with the badge does not exist.");
                }
            }
            else if (string.IsNullOrEmpty(badge.EventId))
            {
                throw new ArgumentException("Event ID cannot be null or empty for a badge.");
            }
            if (string.IsNullOrEmpty(badge.BadgeText))
            {
                throw new ArgumentException("Badge text cannot be null or empty.");
            }
            await _context.SaveChangesAsync();
            return badge;
        }
        public async Task<Badge> UpdateBadgeAsync(string badgeId, Badge updatedBadge)
        {
            var existingBadge = await _context.Badges.FindAsync(badgeId);
            if (existingBadge == null)
            {
                throw new Exception("Badge not found.");
            }
            existingBadge.EventId = updatedBadge.EventId ?? existingBadge.EventId;
            existingBadge.BadgeText = updatedBadge.BadgeText ?? existingBadge.BadgeText;
            existingBadge.IconUrl = updatedBadge.IconUrl ?? existingBadge.IconUrl;
            _context.Badges.Update(existingBadge);
            await _context.SaveChangesAsync();
            return existingBadge;
        }
        public async Task<bool> DeleteBadgeAsync(string badgeId)
        {
            var badge = await _context.Badges.FindAsync(badgeId);
            if (badge == null)
            {
                return false;
            }
            _context.Badges.Remove(badge);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Badge>> GetBadgesByEventIdAsync(string eventId)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                throw new ArgumentException("Event ID cannot be null or empty.");
            }
            if (!await _context.Events.AnyAsync(e => e.EventId == eventId))
            {
                throw new Exception("Event not found.");
            }
            var badges = await _context.Badges
                .Where(b => b.EventId == eventId)
                .ToListAsync();
            if (badges == null || !badges.Any())
            {
                throw new Exception("No badges found for the event.");
            }
            return badges;
        }
        public async Task<List<Badge>> GetBadgesByUserIdAsync(string userId)
        {
            var userBadgeIds = await _context.UserBadges
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.BadgeId)
                .ToListAsync();
            if (userBadgeIds == null || !userBadgeIds.Any())
            {
                throw new Exception("No badges found for the user.");
            }
            var badges = new List<Badge>();
            foreach (var badgeId in userBadgeIds)
            {
                var badge = await _context.Badges.FindAsync(badgeId);
                if (badge != null)
                {
                    badges.Add(badge);
                }
            }
            if (badges.Count == 0)
            {
                throw new Exception("No badges found for the user.");
            }
            return badges;
        }
        public Task<UserBadge> AssignBadgeToUserAsync(string userId, string badgeId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(badgeId))
            {
                throw new ArgumentException("Badge ID cannot be null or empty.");
            }
            var existingUserBadge = _context.UserBadges
                .FirstOrDefault(ub => ub.UserId == userId && ub.BadgeId == badgeId);
            if (existingUserBadge != null)
            {
                throw new Exception("User already has this badge.");
            }
            var badge = _context.Badges.Find(badgeId);
            if (badge == null)
            {
                throw new Exception("Badge not found.");
            }
            if (badge.EventId != null)
            {
                var eventExists = _context.Events.Any(e => e.EventId == badge.EventId);
                if (!eventExists)
                {
                    throw new Exception("Event associated with the badge does not exist.");
                }
            }
            if (!_context.AppUsers.Any(u => u.Id == userId))
            {
                throw new Exception("User not found.");
            }
            var userBadge = new UserBadge
            {
                UserId = userId,
                BadgeId = badgeId
            };
            _context.UserBadges.Add(userBadge);
            return _context.SaveChangesAsync().ContinueWith(_ => userBadge);
        }
        public async Task<bool> RemoveBadgeFromUserAsync(string userId, string badgeId)
        {
            var userBadge = await _context.UserBadges
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BadgeId == badgeId);
            if (userBadge == null)
            {
                return false;
            }
            _context.UserBadges.Remove(userBadge);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
