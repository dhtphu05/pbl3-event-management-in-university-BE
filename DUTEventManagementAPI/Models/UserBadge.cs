namespace DUTEventManagementAPI.Models
{
    public class UserBadge
    {
        public string UserBadgeId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string BadgeId { get; set; } = string.Empty;
    }
}
