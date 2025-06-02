namespace DUTEventManagementAPI.Models
{
    public class EventFacultyScope
    {
        public string EventFacultyScopeId { get; set; } = Guid.NewGuid().ToString();
        public string EventId { get; set; } = string.Empty;
        public string FacultyId { get; set; }= string.Empty;
    }
}
