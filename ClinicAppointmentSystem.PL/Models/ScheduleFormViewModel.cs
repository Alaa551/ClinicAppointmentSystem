using System.ComponentModel.DataAnnotations;

namespace ClinicAppointmentSystem.PL.Models
{
    public class ScheduleFormViewModel
    {
        public int? ScheduleID { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        public int DoctorID { get; set; }

        [Required(ErrorMessage = "Day is required.")]
        [Display(Name = "Day")]
        public DayOfWeek? DayOfWeek { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.Time)]
        [Display(Name = "Start")]
        public TimeSpan? StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.Time)]
        [Display(Name = "End")]
        public TimeSpan? EndTime { get; set; }
    }
}
