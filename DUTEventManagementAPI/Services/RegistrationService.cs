using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Services.Interfaces;
namespace DUTEventManagementAPI.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly AppDbContext _context;
        public RegistrationService(AppDbContext context)
        {
            _context = context;
        }
        public List<Registration> GetAllRegistrations()
        {
            return _context.Registrations.ToList();
        }
        public Registration RegisterUserForEvent(string userId, string eventId)
        {
            var user = _context.AppUsers.Find(userId);
            if (user == null)
                throw new Exception("User not found");

            var existingRegistration = _context.Registrations
                .FirstOrDefault(r => r.UserId == userId && r.EventId == eventId);
            if (existingRegistration != null)
                throw new Exception("User already registered for this event");
            
            var eventToRegister = _context.Events.Find(eventId);
            if (eventToRegister == null)
                throw new Exception("Event not found");
            if (!eventToRegister.IsOpenedForRegistration)
                throw new Exception("Event is not open for registration");

            if (eventToRegister.IsRestricted)
            {
                bool userFacultyIsInEventScope = _context.EventFacultyScopes
                    .Any(s => s.EventId == eventId && s.FacultyId == user.FacultyId);
                if (!userFacultyIsInEventScope)
                    throw new Exception("User is not in attendant scope");
            }

            var registration = new Registration
            {
                UserId = userId,
                EventId = eventId,
            };
            _context.Registrations.Add(registration);
            _context.SaveChanges();
            return registration;
        }
        public List<AppUser> GetUsersRegisteredForEvent(string eventId)
        {
            var registrations = _context.Registrations.Where(r => r.EventId == eventId).ToList();
            var userIds = registrations.Select(r => r.UserId).ToList();
            return _context.AppUsers.Where(u => userIds.Contains(u.Id)).ToList();
        }
        public List<Event> GetEventsUserRegisteredFor(string userId)
        {
            var registrations = _context.Registrations.Where(r => r.UserId == userId).ToList();
            var eventIds = registrations.Select(r => r.EventId).ToList();
            return _context.Events.Where(e => eventIds.Contains(e.EventId)).ToList();
        }
        public bool RemoveRegistration(string eventId, string userId)
        {
            var registration = _context.Registrations
                .FirstOrDefault(r => r.EventId == eventId && r.UserId == userId);
            if (registration == null)
                throw new Exception("Registration not found");
            _context.Registrations.Remove(registration);
            return _context.SaveChanges() > 0;
        }
    }
}
