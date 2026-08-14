using ClinicAppointmentSystem.DAL.Database.Data;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.DAL.Repositories.Implementations
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(ClinicDbContext context) : base(context) { }

        public async Task<Schedule> GetByDoctorAndDayAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.DoctorID == doctorId && s.DayOfWeek == dayOfWeek);
        }
    }
}