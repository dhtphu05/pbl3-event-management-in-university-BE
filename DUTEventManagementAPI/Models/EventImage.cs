namespace DUTEventManagementAPI.Models
{
    public class EventImage
    {
        public string EventImageId { get; set; } = Guid.NewGuid().ToString();
        public string EventId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool isThumbnail { get; set; } = false;
    }
}
