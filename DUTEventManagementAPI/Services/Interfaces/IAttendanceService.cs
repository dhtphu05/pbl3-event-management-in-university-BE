﻿using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IAttendanceService
    {
        List<Attendance> GetAllAttendances();
        Attendance GetAttendanceByRegistrationId(string registrationId);
        List<Event> GetEventsUserMarkedAttendanceFor(string userId);
        List<AppUser> GetUsersMarkedAttendanceForEvent(string eventId);
        Attendance MarkAttendance(string registrationId, double latitude, double longitude);
        Attendance MarkAttendanceByRegistration(string registrationId);
        bool RemoveAttendance(string attendanceId);
    }
}