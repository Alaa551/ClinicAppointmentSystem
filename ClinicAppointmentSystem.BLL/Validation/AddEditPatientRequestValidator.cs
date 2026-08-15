using ClinicAppointmentSystem.BLL.DTOs;
using FluentValidation;

namespace ClinicAppointmentSystem.BLL.Validation
{
    public class AddEditPatientRequestValidator : AbstractValidator<AddEditPatientRequest>
    {
        public AddEditPatientRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Patient name is required.")
                .MaximumLength(100);

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Birth date is required.")
                .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Select a valid gender.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20);

            RuleFor(x => x.Street)
                .MaximumLength(150);

            RuleFor(x => x.City)
                .MaximumLength(100);

            RuleFor(x => x.ZipCode)
                .MaximumLength(20);

            RuleFor(x => x.PatientID)
                .GreaterThanOrEqualTo(0);
        }
    }
}