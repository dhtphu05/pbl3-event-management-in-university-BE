using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public interface IEventCategoryService
    {
        EventCategory CreateEventCategory(string eventId, string categoryName);
        List<EventCategory> GetAllEventCategories();
        List<Category> GetCategoriesOfEvent(string eventId);
        EventCategory GetEventCategoryById(string id);
        List<Event> GetEventsByCategory(string categoryName);
        bool DeleteEventCategory(string eventId, string categoryName);
    }
}