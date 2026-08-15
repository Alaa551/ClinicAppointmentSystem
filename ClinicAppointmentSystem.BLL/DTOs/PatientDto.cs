using ClinicAppointmentSystem.DAL.Enums;
using System;

namespace ClinicAppointmentSystem.BLL.DTOs
{
    public class PatientDto
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}
