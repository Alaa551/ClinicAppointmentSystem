using System;

namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class ScheduleDto
    {
        public int ScheduleID { get; set; }
        public int DoctorID { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
