using ClinicAppointmentSystem.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicAppointmentSystem.BLL.Services.Abstraction
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetByDoctorAsync(int doctorId);
        Task<ScheduleDto> AddAsync(AddEditScheduleRequest request);
        Task<ScheduleDto> EditAsync(AddEditScheduleRequest request);
        Task DeleteAsync(int id);
    }
}
