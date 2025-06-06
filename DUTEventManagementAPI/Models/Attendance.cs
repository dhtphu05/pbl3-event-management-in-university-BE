namespace DUTEventManagementAPI.Models
{
    public class Attendance
    {
        public string AttendanceId { get; set; } = Guid.NewGuid().ToString();
        public string RegistrationId { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime AttendanceDate { get; set; } = DateTime.Now;
    }
}
