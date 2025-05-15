using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface ITimeSlotService
    {
        TimeSlot AddTimeSlot(TimeSlot timeSlot);
        List<TimeSlot> GetAllTimeSlots();
        TimeSlot GetTimeSlotByTime(DateTime time);
        List<TimeSlot> GetTimeSlotsByEventId(string eventId);
        bool RemoveTimeSlot(string id);
    }
}