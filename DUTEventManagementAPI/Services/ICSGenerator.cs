using DUTEventManagementAPI.Models;
using System;
using System.IO;
using System.Text;

namespace DUTEventManagementAPI.Services
{
    public class IcsGenerator
    {
        public static string GenerateIcsFile(Event eventModel)
        {
            // Khởi tạo StringBuilder để xây dựng nội dung .ics
            StringBuilder icsContent = new StringBuilder();

            // Header của VCALENDAR
            icsContent.AppendLine("BEGIN:VCALENDAR");
            icsContent.AppendLine("VERSION:2.0");
            icsContent.AppendLine($"PRODID:-//DUT //Event {eventModel.EventId}//EN");
            icsContent.AppendLine("CALSCALE:GREGORIAN");

            // Múi giờ Asia/Ho_Chi_Minh
            icsContent.AppendLine("BEGIN:VTIMEZONE");
            icsContent.AppendLine("TZID:Asia/Ho_Chi_Minh");
            icsContent.AppendLine($"LAST-MODIFIED:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}");
            icsContent.AppendLine("TZ occupa://www.tzurl.org/zoneinfo-outlook/Asia/Ho_Chi_Minh");
            icsContent.AppendLine("X-LIC-LOCATION:Asia/Ho_Chi_Minh");
            icsContent.AppendLine("BEGIN:STANDARD");
            icsContent.AppendLine("TZNAME:+07");
            icsContent.AppendLine("TZOFFSETFROM:+0700");
            icsContent.AppendLine("TZOFFSETTO:+0700");
            icsContent.AppendLine("DTSTART:19700101T000000");
            icsContent.AppendLine("END:STANDARD");
            icsContent.AppendLine("END:VTIMEZONE");

            // Sự kiện VEVENT
            icsContent.AppendLine("BEGIN:VEVENT");
            icsContent.AppendLine($"DTSTAMP:{DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ")}");
            icsContent.AppendLine($"UID:{Guid.NewGuid()}@dutevent.com");
            icsContent.AppendLine($"DTSTART;TZID=Asia/Ho_Chi_Minh:{eventModel.StartDate.ToString("yyyyMMddTHHmmss")}");
            icsContent.AppendLine($"DTEND;TZID=Asia/Ho_Chi_Minh:{eventModel.EndDate.ToString("yyyyMMddTHHmmss")}");
            icsContent.AppendLine($"SUMMARY:{EscapeString(eventModel.EventName)}");
            icsContent.AppendLine($"DESCRIPTION:{EscapeString(eventModel.Description)}");
            icsContent.AppendLine($"LOCATION:{EscapeString(eventModel.Location)}");
            icsContent.AppendLine("TRANSP:TRANSPARENT");
            icsContent.AppendLine("X-MICROSOFT-CDO-BUSYSTATUS:FREE");
            icsContent.AppendLine("CLASS:PUBLIC");

            // Cảnh báo 1: 1 ngày trước
            icsContent.AppendLine("BEGIN:VALARM");
            icsContent.AppendLine("ACTION:DISPLAY");
            icsContent.AppendLine($"DESCRIPTION:{EscapeString(eventModel.EventName)}");
            icsContent.AppendLine("TRIGGER:-P1D");
            icsContent.AppendLine("END:VALARM");

            // Cảnh báo 2: 2 giờ trước
            icsContent.AppendLine("BEGIN:VALARM");
            icsContent.AppendLine("ACTION:DISPLAY");
            icsContent.AppendLine($"DESCRIPTION:{EscapeString(eventModel.EventName)}");
            icsContent.AppendLine("TRIGGER:-PT2H");
            icsContent.AppendLine("END:VALARM");

            icsContent.AppendLine("END:VEVENT");
            icsContent.AppendLine("END:VCALENDAR");

            return icsContent.ToString();
        }

        public static void SaveIcsFile(string icsContent, string filePath)
        {
            File.WriteAllText(filePath, icsContent);
        }

        // Hàm xử lý các chuỗi để thoát các ký tự đặc biệt theo chuẩn iCalendar
        private static string EscapeString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n");
        }
    }
}