using ClinicAppointmentSystem.DAL.Enums;

namespace ClinicAppointmentSystem.DAL.Database.Entities
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppointmentStatus Status { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
