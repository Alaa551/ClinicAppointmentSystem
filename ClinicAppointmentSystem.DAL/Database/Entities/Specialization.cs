namespace ClinicAppointmentSystem.DAL.Database.Entities
{
    public class Specialization
    {
        public int SpecializationID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
