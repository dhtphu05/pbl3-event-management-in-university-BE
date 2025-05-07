using FluentEmail.Core.Models;
namespace DUTEventManagementAPI.Services
{
    public interface IEmailService
    {
        Task Send(EmailMetadata emailMetaData);
    }
}
