using ClinicAppointmentSystem.DAL.Repositories;

namespace ClinicAppointmentSystem.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
