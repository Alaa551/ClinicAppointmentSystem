using ClinicAppointmentSystem.BLL.DTOs;

namespace ClinicAppointmentSystem.BLL.Services.Abstraction
{
    public interface IDoctorService
    {
        Task<PagedResult<DoctorDto>> GetAllAsync(int pageNumber, int pageSize, string search);
        Task<DoctorDto> GetByIdAsync(int id);
        Task<DoctorDto> AddAsync(AddEditDoctorRequest request);
        Task<DoctorDto> EditAsync(AddEditDoctorRequest request);
        Task DeleteAsync(int id);

        Task<IEnumerable<LookupModel>> SearchActiveDoctorsAutoComplete(string term, int maxResults = 10);
    }
}
