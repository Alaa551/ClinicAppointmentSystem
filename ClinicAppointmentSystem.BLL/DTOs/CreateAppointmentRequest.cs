namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class CreateAppointmentRequest
    {
        public int DoctorID { get; set; }
        public int PatientID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
    }
}