using System.ComponentModel.DataAnnotations;

namespace ClinicAppointmentSystem.PL.Models
{
    public class DoctorFormViewModel
    {
        public int DoctorID { get; set; }

        [Required(ErrorMessage = "Doctor name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [Display(Name = "Full name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        [StringLength(100)]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100)]
        public string Email { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
