namespace DUTEventManagementAPI.Models
{
    public class TimeSlot
    {
        public string TimeSlotId { get; set; } = Guid.NewGuid().ToString();
        public string EventId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Description { get; set; } = string.Empty;
        public DateTime? CreateAt { get; set; } = DateTime.Now;
    }
}
