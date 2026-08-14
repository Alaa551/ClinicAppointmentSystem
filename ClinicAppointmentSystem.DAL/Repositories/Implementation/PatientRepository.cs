using ClinicAppointmentSystem.DAL.Database.Data;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.DAL.Repositories.Implementations
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(ClinicDbContext context) : base(context) { }

        public async Task<IEnumerable<Patient>> SearchPatientsAutoComplete(string term, int maxResults = 11)
        {
            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(p => p.Name.Contains(term) || p.PhoneNumber.Contains(term));
            }

            return await query
                .OrderBy(p => p.Name)
                .Take(maxResults)
                .ToListAsync();
        }
    }
}
