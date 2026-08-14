using ClinicAppointmentSystem.DAL.Database.Entities;

namespace ClinicAppointmentSystem.DAL.Repositories.Abstraction
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<IEnumerable<Patient>> SearchPatientsAutoComplete(string searchTerm, int maxResults = 11);

    }
}
