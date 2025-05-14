namespace DUTEventManagementAPI.Models
{
    public class EventCategory
    {
        public string EventCategoryId { get; set; } = Guid.NewGuid().ToString();
        public string EventId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
    }
}
