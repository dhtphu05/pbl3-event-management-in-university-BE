using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public interface IEmailService
    {
        void SendEmail(Message message);
        Task SendEmailWithTemplateAsync(Event eventModel, string[] recipients, string templatePath, string subject);
    }
}