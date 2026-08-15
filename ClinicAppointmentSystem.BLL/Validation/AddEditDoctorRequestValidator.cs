using ClinicAppointmentSystem.BLL.DTOs;
using FluentValidation;

namespace ClinicAppointmentSystem.BLL.Validation
{
    public class AddEditDoctorRequestValidator : AbstractValidator<AddEditDoctorRequest>
    {
        public AddEditDoctorRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Doctor name is required.")
                .MaximumLength(100);

            RuleFor(x => x.SpecializationID)
                .GreaterThan(0).WithMessage("Specialization is required.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.")
                .MaximumLength(100);

            RuleFor(x => x.DoctorID)
                .GreaterThanOrEqualTo(0);
        }
    }
}
