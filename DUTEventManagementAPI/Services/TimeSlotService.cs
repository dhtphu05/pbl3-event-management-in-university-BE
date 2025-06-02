using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly AppDbContext _context;
        public TimeSlotService(AppDbContext context)
        {
            _context = context;
        }
        public List<TimeSlot> GetAllTimeSlots()
        {
            return _context.TimeSlots.ToList();
        }
        public TimeSlot GetTimeSlotByTime(DateTime time)
        {
            var timeSlot = _context.TimeSlots.FirstOrDefault(ts => ts.StartTime <= time && ts.EndTime >= time);
            if (timeSlot == null)
            {
                throw new Exception("Time slot not found");
            }
            return timeSlot;
        }
        public List<TimeSlot> GetTimeSlotsByEventId(string eventId)
        {
            return _context.TimeSlots.Where(ts => ts.EventId == eventId).ToList();
        }
        public TimeSlot AddTimeSlot(TimeSlot timeSlot)
        {
            var events = _context.Events.ToList();
            var eventExists = events.Any(e => e.EventId == timeSlot.EventId);
            if (!eventExists)
            {
                throw new Exception("Event not found");
            }
            // Check for overlapping time slots of the event
            var overlappingTimeSlots = _context.TimeSlots
                .Where(ts => ts.EventId == timeSlot.EventId &&
                             ((ts.StartTime < timeSlot.EndTime && ts.EndTime > timeSlot.StartTime)))
                .ToList();
            if (overlappingTimeSlots.Count > 0)
            {
                throw new Exception("Time slot overlaps with existing time slots");
            }
            _context.TimeSlots.Add(timeSlot);
            _context.SaveChanges();
            return timeSlot;
        }
        public bool RemoveTimeSlot(string id)
        {
            var timeSlot = _context.TimeSlots.FirstOrDefault(ts => ts.TimeSlotId == id);
            if (timeSlot != null)
            {
                _context.TimeSlots.Remove(timeSlot);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
