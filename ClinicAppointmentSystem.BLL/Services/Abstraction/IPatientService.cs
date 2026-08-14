using ClinicAppointmentSystem.BLL.DTOs;

namespace ClinicAppointmentSystem.BLL.Services.Abstraction
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync();
        Task<PatientDto> GetByIdAsync(int id);
        Task<PatientDto> AddAsync(AddEditPatientRequest request);
        Task<PatientDto> EditAsync(AddEditPatientRequest request);
        Task DeleteAsync(int id);

        Task<IEnumerable<LookupModel>> SearchPatientsAutoComplete(string term);
    }
}