namespace DUTEventManagementAPI.Models
{
    public class Badge
    {
        public string BadgeId { get; set; } = Guid.NewGuid().ToString();
        public string EventId { get; set; } = string.Empty;
        public string BadgeText { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
    }
}
