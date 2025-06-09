using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IEventService _eventService;
        private readonly AppDbContext _context;
        public AttendanceService(AppDbContext context, IEventService eventService)
        {
            _context = context;
            _eventService = eventService;
        }

        public List<Attendance> GetAllAttendances()
        {
            var attendances = _context.Attendances.ToList();
            if (attendances == null || !attendances.Any())
            {
                throw new Exception("No attendances found");
            }
            return attendances;
        }

        public Attendance MarkAttendance(string registrationId, double latitude, double longitude)
        {
            var registration = _context.Registrations.FirstOrDefault(r => r.RegistrationId == registrationId);
            if (registration == null)
                throw new Exception("Registration not found");
            var eventDetails = _context.Events.FirstOrDefault(e => e.EventId == registration.EventId);
            if (eventDetails == null)
                throw new Exception("Event not found");
            bool isInCheckInArea = _eventService.GetDistanceInKm(eventDetails.Latitude, eventDetails.Longitude, latitude, longitude) <= 0.5;
            if (!isInCheckInArea)
                throw new Exception("User is not in the check-in area");

            // Check if the user has already marked attendance for this event
            var existingAttendance = _context.Attendances
                .FirstOrDefault(a => a.RegistrationId == registrationId);
            if (existingAttendance != null)
            {
                throw new Exception("Attendance already marked for this registration");
            }

            var attendance = new Attendance
            {
                RegistrationId = registrationId,
                Latitude = latitude,
                Longitude = longitude
            };

            _context.Attendances.Add(attendance);

            var user = _context.Users.FirstOrDefault(u => u.Id == registration.UserId);
            if (user == null )
                throw new Exception("User not found");
            // if event has a badge, assign it to the user
            var existingBadge = _context.Badges.FirstOrDefault(b => b.EventId == eventDetails.EventId) ?? throw new Exception("Event does not have a badge");
            if (existingBadge != null)
            {
                if(!_context.UserBadges.Any(b => b.UserId == user.Id && b.BadgeId == existingBadge.BadgeId))
                {
                    _context.UserBadges.Add(new UserBadge
                    {
                        UserId = user.Id,
                        BadgeId = existingBadge.BadgeId
                    });
                }
            }
            _context.SaveChanges();
            return attendance;
        }
        public Attendance MarkAttendanceByRegistration(string registrationId)
        {
            var registration = _context.Registrations.FirstOrDefault(r => r.RegistrationId == registrationId);
            if (registration == null)
                throw new Exception("Registration not found");
            var eventDetails = _context.Events.FirstOrDefault(e => e.EventId == registration.EventId);
            if (eventDetails == null)
                throw new Exception("Event not found");

            var existingAttendance = _context.Attendances
                .FirstOrDefault(a => a.RegistrationId == registrationId);
            if (existingAttendance != null)
            {
                throw new Exception("Attendance already marked for this registration");
            }

            var attendance = new Attendance
            {
                RegistrationId = registrationId,
                Latitude = 0,
                Longitude = 0 // Latitude and Longitude are not needed for QR registration
            };

            _context.Attendances.Add(attendance);

            var user = _context.Users.FirstOrDefault(u => u.Id == registration.UserId);
            if (user == null)
                throw new Exception("User not found");
            // if event has a badge, assign it to the user
            var existingBadge = _context.Badges.FirstOrDefault(b => b.EventId == eventDetails.EventId) ?? throw new Exception("Event does not have a badge");
            if (existingBadge != null)
            {
                if (!_context.UserBadges.Any(b => b.UserId == user.Id && b.BadgeId == existingBadge.BadgeId))
                {
                    _context.UserBadges.Add(new UserBadge
                    {
                        UserId = user.Id,
                        BadgeId = existingBadge.BadgeId
                    });
                }
            }

            _context.SaveChanges();
            return attendance;
        }

        public List<AppUser> GetUsersMarkedAttendanceForEvent(string eventId)
        {
            // Join Registrations, Attendances, and AppUsers to get users who marked attendance for a specific event
            return _context.Registrations
                .Where(r => r.EventId == eventId)
                .Join(_context.Attendances,
                    registration => registration.RegistrationId,
                    attendance => attendance.RegistrationId,
                    (registration, attendance) => registration)
                .Join(_context.Users,
                    registration => registration.UserId,
                    user => user.Id,
                    (registration, user) => user)
                .Distinct()
                .ToList();
        }

        public List<Event> GetEventsUserMarkedAttendanceFor(string userId)
        {
            // Join Registrations, Attendances, and Events to get events a user marked attendance for
            return _context.Registrations
                .Where(r => r.UserId == userId)
                .Join(_context.Attendances,
                    registration => registration.RegistrationId,
                    attendance => attendance.RegistrationId,
                    (registration, attendance) => registration)
                .Join(_context.Events,
                    registration => registration.EventId,
                    @event => @event.EventId,
                    (registration, @event) => @event)
                .Distinct()
                .ToList();
        }

        public bool RemoveAttendance(string attendanceId)
        {
            var attendance = _context.Attendances.FirstOrDefault(a => a.AttendanceId == attendanceId);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public Attendance GetAttendanceByRegistrationId(string registrationId)
        {
            var result =_context.Attendances.FirstOrDefault(a => a.RegistrationId == registrationId);
            if (result == null)
            {
                throw new Exception("Attendance not found");
            }
            return result;
        }
    }
}