using ClinicAppointmentSystem.DAL.Database.Data;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.DAL.Repositories.Implementations
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ClinicDbContext context) : base(context) { }

        public async Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(a => a.DoctorID == doctorId && a.AppointmentDate.Date == date.Date)
                .ToListAsync();
        }
    }
}
