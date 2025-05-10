using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public class EventService : IEventService
    {
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;
        public EventService(AppDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }
        public List<Event> GetAllEvents()
        {
            return _context.Events.ToList();
        }
        public async Task<Event?> GetEventByIdAsync(string eventId)
        {
            return await _context.Events.FindAsync(eventId);
        }
        public async Task<CreateEventResponse> CreateEventAsync(Event newEvent)
        {
            var result = new CreateEventResponse();

            // Tìm các sự kiện trong bán kính 1km
            var allEvents = GetAllEvents();
            foreach (var e in allEvents)
            {
                double distance = GetDistanceInKm(e.Latitude, e.Longitude, newEvent.Latitude, newEvent.Longitude);
                bool isTimeOverlap = (newEvent.StartDate < e.StartDate && newEvent.EndDate > e.StartDate)
                                  || (newEvent.StartDate < e.EndDate && newEvent.EndDate > e.EndDate);

                if (distance <= 1.0)
                {
                    if (isTimeOverlap)
                        result.Errors.Add("Trùng địa điểm và khung giờ với một sự kiện khác.");
                }
                if (isTimeOverlap && e.HostId == newEvent.HostId)
                {
                    result.Errors.Add("Người tổ chức đã có sự kiện khác trong cùng khung giờ.");
                }
            }

            // Nếu có lỗi thì trả về
            if (result.Errors.Count > 0)
            {
                result.Succeeded = false;
                return result;
            }

            // Tạo sự kiện nếu hợp lệ
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            result.Succeeded = true;
            return result;
        }
        private double GetDistanceInKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double R = 6371; // bán kính trái đất (km)
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double deg) => deg * (Math.PI / 180);
        public Event UpdateEventAsync(string eventId, Event updatedEvent)
        {
            var eventToUpdate = _context.Events.Find(eventId);
            if (eventToUpdate == null)
            {
                Console.WriteLine("Event not found");
                throw new Exception("Event not found");
            } 
            
            eventToUpdate.EventName = updatedEvent.EventName ?? eventToUpdate.EventName;
            eventToUpdate.Description = updatedEvent.Description ?? eventToUpdate.Description;
            eventToUpdate.AttendanceType = updatedEvent.AttendanceType ?? eventToUpdate.AttendanceType;
            eventToUpdate.Location = updatedEvent.Location ?? eventToUpdate.Location;
            eventToUpdate.Longitude = updatedEvent.Longitude != 0 ? updatedEvent.Longitude : eventToUpdate.Longitude;
            eventToUpdate.Latitude = updatedEvent.Latitude != 0 ? updatedEvent.Latitude : eventToUpdate.Latitude;
            eventToUpdate.StartDate = updatedEvent.StartDate != DateTime.MinValue ? updatedEvent.StartDate : eventToUpdate.StartDate;
            eventToUpdate.EndDate = updatedEvent.EndDate != DateTime.MinValue ? updatedEvent.EndDate : eventToUpdate.EndDate;
            eventToUpdate.Capacity = updatedEvent.Capacity != 0 ? updatedEvent.Capacity : eventToUpdate.Capacity;
            eventToUpdate.HostId = updatedEvent.HostId ?? eventToUpdate.HostId;
            eventToUpdate.LogoUrl = updatedEvent.LogoUrl ?? eventToUpdate.LogoUrl;
            eventToUpdate.CoverUrl = updatedEvent.CoverUrl ?? eventToUpdate.CoverUrl;
            eventToUpdate.PlanLink = updatedEvent.PlanLink ?? eventToUpdate.PlanLink;
            _context.Events.Update(eventToUpdate);
            _context.SaveChanges();
            return eventToUpdate;

        }
        public async Task<bool> DeleteEventAsync(string eventId)
        {
            var eventToDelete = await GetEventByIdAsync(eventId);
            if (eventToDelete == null) return false;
            _context.Events.Remove(eventToDelete);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
