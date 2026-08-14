using ClinicAppointmentSystem.BLL.DTOs;
using FluentValidation;

namespace ClinicAppointmentSystem.BLL.Validation
{
    public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
    {
        public CreateAppointmentRequestValidator()
        {
            RuleFor(x => x.DoctorID)
                .GreaterThan(0).WithMessage("A doctor must be selected.");

            RuleFor(x => x.PatientID)
                .GreaterThan(0).WithMessage("A patient must be selected.");

            RuleFor(x => x.AppointmentDate)
                .NotEmpty().WithMessage("Appointment date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Appointment date cannot be in the past.");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("A time slot must be selected.");
        }
    }
}