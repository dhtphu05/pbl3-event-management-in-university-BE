using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}