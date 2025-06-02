namespace DUTEventManagementAPI.Models
{
    public class Faculty
    {
        public string FacultyId { get; set; } = Guid.NewGuid().ToString();
        public string FacultyName { get; set; } = string.Empty;
    }
}
