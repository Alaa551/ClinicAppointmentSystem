using ClinicAppointmentSystem.DAL.Enums;
using System;

namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class AppointmentDto
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
    }
}
