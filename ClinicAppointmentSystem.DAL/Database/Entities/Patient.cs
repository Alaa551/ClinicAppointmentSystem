using ClinicAppointmentSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace ClinicAppointmentSystem.DAL.Database.Entities
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
