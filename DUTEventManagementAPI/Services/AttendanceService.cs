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

        public Attendance MarkAttendance(string registrationId, double latitude, double longitude)
        {
            var registration = _context.Registrations.FirstOrDefault(r => r.RegistrationId == registrationId)
                ?? throw new Exception("Registration not found");

            var eventDetails = _context.Events.FirstOrDefault(e => e.EventId == registration.EventId)
                ?? throw new Exception("Event not found");

            bool isInCheckInArea = _eventService.GetDistanceInKm(eventDetails.Latitude, eventDetails.Longitude, latitude, longitude) <= 0.5;
            if (!isInCheckInArea)
                throw new Exception("User is not in the check-in area");

            if (_context.Attendances.Any(a => a.RegistrationId == registrationId))
                throw new Exception("Attendance already marked for this registration");

            var attendance = new Attendance
            {
                RegistrationId = registrationId,
                Latitude = latitude,
                Longitude = longitude,
                AttendanceDate = DateTime.UtcNow
            };
            _context.Attendances.Add(attendance);
            _context.SaveChanges();

            TryAssignBadgeAfterAttendance(registration);
            return attendance;
        }

        public Attendance MarkAttendanceByRegistration(string registrationId)
        {
            var registration = _context.Registrations.FirstOrDefault(r => r.RegistrationId == registrationId)
                ?? throw new Exception("Registration not found");

            if (_context.Attendances.Any(a => a.RegistrationId == registrationId))
                throw new Exception("Attendance already marked for this registration");

            var attendance = new Attendance
            {
                RegistrationId = registrationId,
                Latitude = 0,
                Longitude = 0,
                AttendanceDate = DateTime.UtcNow
            };
            _context.Attendances.Add(attendance);
            _context.SaveChanges();

            TryAssignBadgeAfterAttendance(registration);
            return attendance;
        }

        private void TryAssignBadgeAfterAttendance(Registration registration)
        {
            try
            {
                var badge = _context.Badges.FirstOrDefault(b => b.EventId == registration.EventId);

                if (badge != null)
                {
                    bool alreadyHasBadge = _context.UserBadges.Any(ub => ub.UserId == registration.UserId && ub.BadgeId == badge.BadgeId);
                    if (!alreadyHasBadge)
                    {
                        var userBadge = new UserBadge { UserId = registration.UserId, BadgeId = badge.BadgeId };
                        _context.UserBadges.Add(userBadge);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi để bạn thấy
                Console.WriteLine($"CRITICAL error during badge assignment for user {registration.UserId} and event {registration.EventId}: {ex.ToString()}");

                // Ném lại lỗi để API trả về mã 500, giúp bạn thấy lỗi trên Postman hoặc trình duyệt
                throw;
            }
        }

        public List<Attendance> GetAllAttendances()
        {
            return _context.Attendances.ToList();
        }

        // === SỬA LẠI PHƯƠNG THỨC NÀY, QUAY VỀ DÙNG JOIN ===
        public List<AppUser> GetUsersMarkedAttendanceForEvent(string eventId)
        {
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

        // === SỬA LẠI PHƯƠNG THỨC NÀY, QUAY VỀ DÙNG JOIN ===
        public List<Event> GetEventsUserMarkedAttendanceFor(string userId)
        {
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
            var result = _context.Attendances.FirstOrDefault(a => a.RegistrationId == registrationId)
                ?? throw new Exception("Attendance not found");
            return result;
        }
    }
}
