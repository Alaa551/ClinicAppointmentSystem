using ClinicAppointmentSystem.DAL.Enums;
using System;

namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class AddEditPatientRequest
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
