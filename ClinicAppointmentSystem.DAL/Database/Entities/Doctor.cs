namespace ClinicAppointmentSystem.DAL.Database.Entities
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        public string Name { get; set; }
        public int SpecializationID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        public Specialization Specialization { get; set; }
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
