using ClinicAppointmentSystem.BLL.DTOs;

namespace ClinicAppointmentSystem.BLL.Services.Abstraction
{
    public interface IAppointmentService
    {
        Task<PagedResult<AppointmentDto>> GetAllAsync(int pageNumber, int pageSize, string search);
        Task<AppointmentDto> GetByIdAsync(int id);
        Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, DateTime date, int? excludeAppointmentId = null);
        Task<AppointmentDto> CreateAppointmentAsync(AddEditAppointmentRequest request);
        Task<AppointmentDto> EditAppointmentAsync(AddEditAppointmentRequest request);
        Task CancelAsync(int id);
        Task DeleteAsync(int id);
    }
}
