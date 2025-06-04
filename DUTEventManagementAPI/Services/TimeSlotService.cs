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
        public TimeSlot GetTimeSlotByTitle(string searchString)
        {
            var timeSlot = _context.TimeSlots.FirstOrDefault(ts => ts.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase));
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
        public bool UpdateTimeSlot(string TimeSlotId, TimeSlot timeSlotToUpdate)
        {
            try
            {
                var existingTimeSlot = _context.TimeSlots.FirstOrDefault(ts => ts.TimeSlotId == TimeSlotId);
                if (existingTimeSlot == null)
                {
                    throw new Exception("Time slot not found");
                }
                // Check for overlapping time slots of the event
                var overlappingTimeSlots = _context.TimeSlots
                    .Where(ts => ts.EventId == existingTimeSlot.EventId &&
                                 ts.TimeSlotId != TimeSlotId &&
                                 ((ts.StartTime < timeSlotToUpdate.EndTime && ts.EndTime > timeSlotToUpdate.StartTime)))
                    .ToList();
                if (overlappingTimeSlots.Count > 0)
                {
                    throw new Exception("Updated time slot overlaps with existing time slots");
                }
                existingTimeSlot.Title = timeSlotToUpdate.Title ?? existingTimeSlot.Title;
                existingTimeSlot.StartTime = timeSlotToUpdate.StartTime != DateTime.MinValue ? timeSlotToUpdate.StartTime : existingTimeSlot.StartTime;
                existingTimeSlot.EndTime = timeSlotToUpdate.EndTime != DateTime.MinValue ? timeSlotToUpdate.EndTime : existingTimeSlot.EndTime;
                existingTimeSlot.Description = timeSlotToUpdate.Description ?? existingTimeSlot.Description;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating time slot: {ex.Message}");
            }
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
