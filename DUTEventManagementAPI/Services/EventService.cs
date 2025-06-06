using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;

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

            // Tìm các sự kiện trong bán kính 500m
            var allEvents = GetAllEvents();
            foreach (var e in allEvents)
            {
                double distance = GetDistanceInKm(e.Latitude, e.Longitude, newEvent.Latitude, newEvent.Longitude);
                bool isTimeOverlap = (newEvent.StartDate < e.StartDate && newEvent.EndDate > e.StartDate)
                                  || (newEvent.StartDate < e.EndDate && newEvent.EndDate > e.EndDate);

                if (distance <= 0.5)
                {
                    if (isTimeOverlap)
                        throw new Exception("Trùng địa điểm và khung giờ với một sự kiện khác.");
                }
                if (isTimeOverlap && e.HostId == newEvent.HostId)
                {
                    throw new Exception("Bạn đã có một sự kiện khác trùng khung giờ với sự kiện này.");
                }
            }

            if (result.Errors.Count > 0)
            {
                result.Succeeded = false;
                return result;
            }

            var host = _context.Users.Find(newEvent.HostId);
            if (host == null)
            {
                throw new Exception("Người tổ chức không tồn tại trong hệ thống.");
            }
            // Kiểm tra scope
            if (newEvent.Scope == "private")
            {
                _context.EventFacultyScopes.Add(new EventFacultyScope
                {
                    EventId = newEvent.EventId,
                    FacultyId = host.FacultyId
                });
            }

            // Tạo sự kiện nếu hợp lệ
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            result.Succeeded = true;
            return result;
        }
        public double GetDistanceInKm(double lat1, double lng1, double lat2, double lng2)
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

        public double ToRadians(double deg) => deg * (Math.PI / 180);
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
            eventToUpdate.IsOpenedForRegistration = updatedEvent.IsOpenedForRegistration != eventToUpdate.IsOpenedForRegistration ? updatedEvent.IsOpenedForRegistration : eventToUpdate.IsOpenedForRegistration;
            eventToUpdate.IsCancelled = updatedEvent.IsCancelled != eventToUpdate.IsCancelled ? updatedEvent.IsCancelled : eventToUpdate.IsCancelled;
            eventToUpdate.IsRestricted = updatedEvent.IsRestricted != eventToUpdate.IsRestricted ? updatedEvent.IsRestricted : eventToUpdate.IsRestricted;
            eventToUpdate.Scope = updatedEvent.Scope ?? eventToUpdate.Scope;
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
        public async Task<bool> AddFacultyToScope(string eventId, string facultyId)
        {
            var eventToUpdate = await GetEventByIdAsync(eventId);
            if (eventToUpdate == null) return false;
            if (eventToUpdate.Scope == "public" || eventToUpdate.Scope == "private")
            {
                throw new Exception("Không thể thêm khoa vào phạm vi của sự kiện phạm vi custom");
            }
            if (eventToUpdate.Scope.Contains(facultyId))
            {
                throw new Exception("Khoa đã được thêm vào phạm vi của sự kiện này trước đó.");
            }
            var scope = new EventFacultyScope
            {
                EventId = eventId,
                FacultyId = facultyId
            };
            _context.EventFacultyScopes.Add(scope);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> RemoveFacultyFromScope(string eventId, string facultyId)
        {
            var scope = _context.EventFacultyScopes
                .FirstOrDefault(s => s.EventId == eventId && s.FacultyId == facultyId);
            if (scope == null) return false;
            _context.EventFacultyScopes.Remove(scope);
            return await _context.SaveChangesAsync() > 0;
        }
        public List<Faculty> GetFacultiesInScope(string eventId)
        {
            var facultyIds = _context.EventFacultyScopes
                .Where(s => s.EventId == eventId)
                .Select(s => s.FacultyId)
                .ToList();
            if (facultyIds.Count == 0)
            {
                throw new Exception("No faculties found in the scope of this event.");
            }
            var faculties = new List<Faculty>();
            foreach (var facultyId in facultyIds)
            {
                var faculty = _context.Faculties.Find(facultyId);
                if (faculty == null)
                {
                    throw new Exception("Faculty not found with id " + facultyId);
                }
                faculties.Add(faculty);
            }
            return faculties;
        }

        public async Task<bool> CancelEvent(string eventId)
        {
            var eventToCancel = _context.Events.Find(eventId);
            if (eventToCancel == null)
            {
                throw new Exception("Event not found");
            }
            eventToCancel.IsCancelled = true;
            _context.Events.Update(eventToCancel);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> OpenEventForRegistration(string eventId)
        {
            var eventToOpen = _context.Events.Find(eventId);
            if (eventToOpen == null)
            {
                throw new Exception("Event not found");
            }
            if (eventToOpen.IsCancelled)
            {
                throw new Exception("Cannot open registration for a cancelled event");
            }
            eventToOpen.IsOpenedForRegistration = true;
            _context.Events.Update(eventToOpen);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CloseEventForRegistration(string eventId)
        {
            var eventToClose = _context.Events.Find(eventId);
            if (eventToClose == null)
            {
                throw new Exception("Event not found");
            }
            if (eventToClose.IsCancelled)
            {
                throw new Exception("Cannot close registration for a cancelled event");
            }
            eventToClose.IsOpenedForRegistration = false;
            _context.Events.Update(eventToClose);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
