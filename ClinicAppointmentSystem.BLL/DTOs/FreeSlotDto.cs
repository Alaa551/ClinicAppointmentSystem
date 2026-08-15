using System;

namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class FreeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
