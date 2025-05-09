namespace DUTEventManagementAPI.Services
{
    public class CreateEventResponse
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
