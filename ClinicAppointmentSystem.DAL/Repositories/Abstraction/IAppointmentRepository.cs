using ClinicAppointmentSystem.DAL.Database.Entities;

namespace ClinicAppointmentSystem.DAL.Repositories.Abstraction
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByDoctorAndDateAsync(int doctorId, DateTime date);
    }
}
