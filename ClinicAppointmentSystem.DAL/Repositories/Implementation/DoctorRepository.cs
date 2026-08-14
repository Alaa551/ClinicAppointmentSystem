using ClinicAppointmentSystem.DAL.Database.Data;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.DAL.Repositories.Implementations
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ClinicDbContext context) : base(context) { }

        public async Task<Doctor> GetWithScheduleAsync(int doctorId)
        {
            return await _dbSet
                .Include(d => d.Schedules)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DoctorID == doctorId);
        }

        public async Task<IEnumerable<Doctor>> SearchActiveDoctorsAutoComplete(string searchTerm, int maxResults = 11)
        {
            var query = _dbSet.AsNoTracking().Where(d => d.IsActive);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.Name.Contains(searchTerm));
            }

            return await query
                .OrderBy(d => d.Name)
                .Take(maxResults)
                .ToListAsync();
        }
    }
}
