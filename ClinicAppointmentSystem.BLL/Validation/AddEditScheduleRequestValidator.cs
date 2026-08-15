using ClinicAppointmentSystem.BLL.DTOs;
using FluentValidation;

namespace ClinicAppointmentSystem.BLL.Validation
{
    public class AddEditScheduleRequestValidator : AbstractValidator<AddEditScheduleRequest>
    {
        public AddEditScheduleRequestValidator()
        {
            RuleFor(x => x.DoctorID)
                .GreaterThan(0).WithMessage("Doctor is required.");

            RuleFor(x => x.DayOfWeek)
                .IsInEnum().WithMessage("Select a valid day.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("Start time is required.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("End time is required.")
                .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");
        }
    }
}
