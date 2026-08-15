using ClinicAppointmentSystem.DAL.Database.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClinicAppointmentSystem.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ClinicDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ClinicDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll() => _dbSet.AsNoTracking();

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate) => _dbSet.AsNoTracking().Where(predicate);

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);
    }
}
