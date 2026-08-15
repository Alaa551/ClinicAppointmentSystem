namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class AddEditDoctorRequest
    {
        public int DoctorID { get; set; }
        public string Name { get; set; }
        public int SpecializationID { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
