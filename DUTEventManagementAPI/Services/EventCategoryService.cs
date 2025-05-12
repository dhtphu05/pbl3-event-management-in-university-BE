using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;
namespace DUTEventManagementAPI.Services
{
    public class EventCategoryService : IEventCategoryService
    {
        private readonly AppDbContext _context;
        public EventCategoryService(AppDbContext context)
        {
            _context = context;
        }
        public List<EventCategory> GetAllEventCategories()
        {
            return _context.EventCategories.ToList();
        }
        public EventCategory GetEventCategoryById(string id)
        {
            var eventCategory = _context.EventCategories.FirstOrDefault(ec => ec.EventCategoryId == id);
            if (eventCategory == null)
            {
                throw new Exception("Event category not found");
            }
            return eventCategory;
        }
        public EventCategory CreateEventCategory(string eventId, string categoryName)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
            if (category == null)
            {
                throw new Exception("Category not found to add to event");
            }
            var existingEventCategory = _context.EventCategories
                .FirstOrDefault(ec => ec.EventId == eventId && ec.CategoryId == category.CategoryId);
            if (existingEventCategory != null)
            {
                throw new Exception("Event category already exists");
            }
            var categoryId = category.CategoryId;
            var eventCategory = new EventCategory
            {
                EventId = eventId,
                CategoryId = categoryId
            };
            _context.EventCategories.Add(eventCategory);
            _context.SaveChanges();
            return eventCategory;
        }
        public List<Event> GetEventsByCategory(string categoryName)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            var categoryId = category.CategoryId;
            var eventCategories = _context.EventCategories.Where(ec => ec.CategoryId == categoryId).ToList();
            var events = new List<Event>();
            foreach (var eventCategory in eventCategories)
            {
                var eventItem = _context.Events.FirstOrDefault(e => e.EventId == eventCategory.EventId);
                if (eventItem != null)
                {
                    events.Add(eventItem);
                }
            }
            return events;
        }
        public List<Category> GetCategoriesOfEvent(string eventId)
        {
            var eventCategories = _context.EventCategories.Where(ec => ec.EventId == eventId).ToList();
            var categories = new List<Category>();
            foreach (var eventCategory in eventCategories)
            {
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == eventCategory.CategoryId);
                if (category != null)
                {
                    categories.Add(category);
                }
            }
            return categories;
        }
        public bool DeleteEventCategory(string eventId, string categoryName)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryName == categoryName);
            if (category == null)
            {
                throw new Exception("Category not found");
            }
            var categoryId = category.CategoryId;
            var eventCategory = _context.EventCategories.FirstOrDefault(ec => ec.EventId == eventId && ec.CategoryId == categoryId);
            if (eventCategory == null)
            {
                throw new Exception("Event category not found");
            }
            _context.EventCategories.Remove(eventCategory);
            _context.SaveChanges();
            return true;
        }
    }
}
