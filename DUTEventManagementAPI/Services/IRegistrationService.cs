using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public interface IRegistrationService
    {
        List<Registration> GetAllRegistrations();
        List<Event> GetEventsUserRegisteredFor(string userId);
        List<AppUser> GetUsersRegisteredForEvent(string eventId);
        Registration RegisterUserForEvent(string userId, string eventId);
        bool RemoveRegistration(string eventId, string userId);
    }
}