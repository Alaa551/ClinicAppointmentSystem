using ClinicAppointmentSystem.DAL.Repositories;
using ClinicAppointmentSystem.DAL.Repositories.Abstraction;

namespace ClinicAppointmentSystem.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IDoctorRepository Doctors { get; }
        IScheduleRepository Schedules { get; }
        IPatientRepository Patients { get; }
        IAppointmentRepository Appointments { get; }

        Task<int> SaveChangesAsync();
    }
}
