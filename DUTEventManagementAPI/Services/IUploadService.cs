namespace DUTEventManagementAPI.Services
{
    public interface IUploadService
    {
        Task<UploadResponse> SaveFileAsync(IFormFile file);
    }
}