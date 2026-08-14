using ClinicAppointmentSystem.BLL.DTOs;

namespace ClinicAppointmentSystem.BLL.Services.Abstraction
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllAsync();
        Task<DoctorDto> GetByIdAsync(int id);
        Task<DoctorDto> AddAsync(AddEditDoctorRequest request);
        Task<DoctorDto> EditAsync(AddEditDoctorRequest request);
        Task DeleteAsync(int id);

        Task<IEnumerable<LookupModel>> SearchActiveDoctorsAutoComplete(string term);
    }
}