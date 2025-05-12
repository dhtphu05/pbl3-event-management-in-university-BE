using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public interface IEventService
    {
        Task<CreateEventResponse> CreateEventAsync(Event newEvent);
        Task<bool> DeleteEventAsync(string eventId);
        List<Event> GetAllEvents();
        Task<Event?> GetEventByIdAsync(string eventId);
        Event UpdateEventAsync(string eventId, Event updatedEvent);
        double GetDistanceInKm(double lat1, double lng1, double lat2, double lng2);
    }
}