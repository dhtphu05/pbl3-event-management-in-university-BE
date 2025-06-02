namespace DUTEventManagementAPI.Models
{
    public class Event
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AttendanceType { get; set; } = string.Empty; //online or offline
        public string Location { get; set; } = "54 Nguyễn Lương Bằng, Liên Chiểu, Đà Nẵng";
        public double Longitude { get; set; } = 16.0748;
        public double Latitude { get; set; } = 108.152;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; } = 100;
        public string HostId { get; set; } = string.Empty;
        public bool IsOpenedForRegistration { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public string Status // Upcoming, Ongoing, Ended, Cancelled and Open for Registration
        {
            get
            {
                if (IsCancelled)
                    return "Cancelled";
                if (IsOpenedForRegistration)
                    return "Open for Registration";
                var now = DateTime.UtcNow;
                if (now < StartDate)
                    return "Upcoming";
                else if (now >= StartDate && now <= EndDate)
                    return "Ongoing";
                else
                    return "Ended";
            }
        }
        public bool IsRestricted { get; set; } = false; // true or false
        public string Scope { get; set; } = "public"; // public, private, custom
        public string LogoUrl { get; set; } = string.Empty;
        public string CoverUrl { get; set; } = string.Empty;
        public string PlanLink { get; set; } = string.Empty;
    }
}
