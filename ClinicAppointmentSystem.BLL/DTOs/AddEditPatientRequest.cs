using System;

namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class AddEditPatientRequest
    {
        // 0 on create, populated on update
        public int PatientID { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
