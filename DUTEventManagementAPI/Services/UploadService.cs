using DUTEventManagementAPI.Services.Interfaces;
using System;

namespace DUTEventManagementAPI.Services
{
    public class UploadService : IUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadsFolder = "Uploads";

        public UploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<UploadResponse> SaveFileAsync(IFormFile file)
        {
            var result = new UploadResponse();

            if (file == null || file.Length == 0)
            {
                result.Errors.Add("File is empty.");
                return result;
            }

            try
            {
                var uploadsPath = Path.Combine(_env.ContentRootPath, _uploadsFolder);
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                result.SaveSucceeded = true;
                result.SaveUrl = $"/{_uploadsFolder}/{fileName}";
            }
            catch (Exception ex)
            {
                result.Errors.Add("Internal error: " + ex.Message);
            }

            return result;
        }
    }
}
