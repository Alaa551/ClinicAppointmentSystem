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

            RuleFor(x => x.Specialization)
                .NotEmpty().WithMessage("Specialization is required.")
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Enter a valid email address.")
                .MaximumLength(100);

            // DoctorID only matters on update - 0 means creating new, which is valid
            RuleFor(x => x.DoctorID)
                .GreaterThanOrEqualTo(0);
        }
    }
}
