using ClinicAppointmentSystem.DAL.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAppointmentSystem.PL.Models
{
    public class PatientFormViewModel
    {
        public int PatientID { get; set; }

        [Required(ErrorMessage = "Patient name is required.")]
        [StringLength(100)]
        [Display(Name = "Full name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Birth date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
    }
}
