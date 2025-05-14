using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public interface IEventImageService
    {
        bool AddEventImage(EventImage eventImage);
        bool DeleteEventImage(string eventImageId);
        List<EventImage> GetAllEventImages();
        List<EventImage> GetAllImagesOfAnEvent(string eventId);
    }
}