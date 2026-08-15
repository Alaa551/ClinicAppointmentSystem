using ClinicAppointmentSystem.BLL.DTOs;

namespace ClinicAppointmentSystem.BLL.Services.Abstraction
{
    public interface ISpecializationService
    {
        Task<IEnumerable<LookupModel>> GetActiveAsync();
    }
}
