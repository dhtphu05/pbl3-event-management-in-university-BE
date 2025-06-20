﻿using DUTEventManagementAPI.Data;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;

namespace DUTEventManagementAPI.Services
{
    public class EventImageService : IEventImageService
    {
        private readonly AppDbContext _context;
        public EventImageService(AppDbContext context)
        {
            _context = context;
        }
        public List<EventImage> GetAllEventImages()
        {
            var eventImages = _context.EventImages.ToList();
            if (eventImages == null)
            {
                throw new Exception("Event images not found");
            }
            return eventImages;
        }
        public List<EventImage> GetAllImagesOfAnEvent(string eventId)
        {
            var eventImages = _context.EventImages.Where(e => e.EventId == eventId).ToList();
            if (eventImages == null)
            {
                throw new Exception("Event images not found");
            }
            return eventImages;
        }

        public bool AddEventImage(EventImage eventImage)
        {
            _context.EventImages.Add(eventImage);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteEventImage(string eventImageId)
        {
            var eventImage = _context.EventImages.FirstOrDefault(e => e.EventImageId == eventImageId);
            if (eventImage == null)
            {
                throw new Exception("Event image not found");
            }
            // Delete the image file from the server if it exists

            _context.EventImages.Remove(eventImage);
            return _context.SaveChanges() > 0;
        }
    }
}
