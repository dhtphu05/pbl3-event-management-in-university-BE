namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IUploadService
    {
        Task<UploadResponse> SaveFileAsync(IFormFile file);
    }
}