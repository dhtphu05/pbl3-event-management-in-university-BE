using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IEventService
    {
        Task<CreateEventResponse> CreateEventAsync(Event newEvent);
        Task<bool> DeleteEventAsync(string eventId);
        List<Event> GetAllEvents();
        Task<Event?> GetEventByIdAsync(string eventId);
        Event UpdateEventAsync(string eventId, Event updatedEvent);
        double GetDistanceInKm(double lat1, double lng1, double lat2, double lng2);
        Task<bool> AddFacultyToScope(string eventId, string facultyId);
        Task<bool> RemoveFacultyFromScope(string eventId, string facultyId);
        List<Faculty> GetFacultiesInScope(string eventId);
        Task<bool> CancelEvent(string eventId);
        Task<bool> OpenEventForRegistration(string eventId);
        Task<bool> CloseEventForRegistration(string eventId);
    }
}