using ClinicAppointmentSystem.DAL.Database.Entities;

namespace ClinicAppointmentSystem.DAL.Repositories.Abstraction
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<Doctor> GetWithScheduleAsync(int doctorId);

        Task<IEnumerable<Doctor>> SearchActiveDoctorsAutoComplete(string searchTerm, int maxResults = 11);
    }
}
