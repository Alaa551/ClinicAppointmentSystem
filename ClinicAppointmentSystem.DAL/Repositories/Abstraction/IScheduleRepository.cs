using ClinicAppointmentSystem.DAL.Database.Entities;

namespace ClinicAppointmentSystem.DAL.Repositories.Abstraction
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<Schedule> GetByDoctorAndDayAsync(int doctorId, DayOfWeek dayOfWeek);
    }
}