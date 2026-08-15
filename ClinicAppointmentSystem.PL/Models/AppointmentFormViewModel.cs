using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAppointmentSystem.PL.Models
{
    public class AppointmentFormViewModel
    {
        public int AppointmentID { get; set; }

        [Required(ErrorMessage = "A doctor must be selected.")]
        [Range(1, int.MaxValue, ErrorMessage = "A doctor must be selected.")]
        [Display(Name = "Doctor")]
        public int DoctorID { get; set; }

        [Required(ErrorMessage = "A patient must be selected.")]
        [Range(1, int.MaxValue, ErrorMessage = "A patient must be selected.")]
        [Display(Name = "Patient")]
        public int PatientID { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime? AppointmentDate { get; set; }

        [Required(ErrorMessage = "A time slot must be selected.")]
        [Display(Name = "Time slot")]
        public string StartTime { get; set; }
    }
}
