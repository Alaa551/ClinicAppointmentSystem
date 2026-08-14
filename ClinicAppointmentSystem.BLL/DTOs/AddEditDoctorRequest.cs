namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class AddEditDoctorRequest
    {
        // 0 on create, populated on update
        public int DoctorID { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
