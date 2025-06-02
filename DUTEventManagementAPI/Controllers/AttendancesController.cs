using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace DUTEventManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public AttendancesController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }
        [HttpGet]
        public IActionResult GetAllAttendances()
        {
            var attendances = _attendanceService.GetAllAttendances();
            if (attendances == null || attendances.Count == 0)
            {
                return NotFound("No attendances found");
            }
            return Ok(attendances);
        }
        [HttpGet("{registrationId}")]
        public IActionResult GetAttendancesByRegistrationId(string registrationId)
        {
            var attendances = _attendanceService.GetAttendanceByRegistrationId(registrationId);
            if (attendances == null)
            {
                return NotFound("No attendances found for this registration ID");
            }
            return Ok(attendances);
        }
        [HttpPost("Mark")]
        public IActionResult MarkAttendance([FromBody] Attendance attendance)
        {
            if (attendance == null || string.IsNullOrEmpty(attendance.RegistrationId))
            {
                return BadRequest("Invalid attendance data");
            }
            try
            {
                var markedAttendance = _attendanceService.MarkAttendance(attendance.RegistrationId, attendance.Latitude, attendance.Longitude);
                return CreatedAtAction(nameof(GetAllAttendances), new { id = markedAttendance.AttendanceId }, markedAttendance);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [Authorize(Roles = "Organizer")]
        [HttpPost("MarkByQRRegistration/{registrationId}")]
        public IActionResult MarkAttendanceByQR(string registrationId)
        {
            if (string.IsNullOrEmpty(registrationId))
            {
                return BadRequest("Registration ID is required");
            }
            try
            {
                var markedAttendance = _attendanceService.MarkAttendanceByRegistration(registrationId); // latitude and longitude are not needed for QR registration
                return CreatedAtAction(nameof(GetAllAttendances), new { id = markedAttendance.AttendanceId }, markedAttendance);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet("Users/{eventId}")]
        public IActionResult GetUsersMarkedAttendanceForEvent(string eventId)
        {
            var users = _attendanceService.GetUsersMarkedAttendanceForEvent(eventId);
            if (users == null || users.Count == 0)
            {
                return NotFound("No users marked attendance for this event");
            }
            return Ok(users);
        }
        [HttpGet("Events/{userId}")]
        public IActionResult GetEventsUserMarkedAttendanceFor(string userId)
        {
            var events = _attendanceService.GetEventsUserMarkedAttendanceFor(userId);
            if (events == null || events.Count == 0)
            {
                return NotFound("No events marked attendance for this user");
            }
            return Ok(events);
        }
        [HttpDelete("{attendanceId}")]
        public IActionResult RemoveAttendance(string attendanceId)
        {
            if (string.IsNullOrEmpty(attendanceId))
            {
                return BadRequest("Attendance ID is required");
            }
            var result = _attendanceService.RemoveAttendance(attendanceId);
            if (!result)
            {
                return NotFound("Attendance not found");
            }
            return NoContent();
        }
    }
}
