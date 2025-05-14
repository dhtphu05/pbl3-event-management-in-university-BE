namespace DUTEventManagementAPI.Services
{
    public class UploadResponse
    {
        public bool SaveSucceeded { get; set; }
        public string SaveUrl { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
