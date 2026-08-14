using ClinicAppointmentSystem.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicAppointmentSystem.BLL.Services.Abstraction
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync();
        Task<AppointmentDto> GetByIdAsync(int id);
        Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, DateTime date);
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentRequest request);
        Task CancelAsync(int id);
        Task DeleteAsync(int id);
    }
}
