using ClinicAppointmentSystem.DAL.Database.Data;
using ClinicAppointmentSystem.DAL.Repositories;
using ClinicAppointmentSystem.DAL.Repositories.Abstraction;
using ClinicAppointmentSystem.DAL.Repositories.Implementations;


namespace ClinicAppointmentSystem.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext _context;

        private IDoctorRepository _doctors;
        private IScheduleRepository _schedules;
        private IPatientRepository _patients;
        private IAppointmentRepository _appointments;

        public UnitOfWork(ClinicDbContext context)
        {
            _context = context;
        }

        public IDoctorRepository Doctors =>
            _doctors ??= new DoctorRepository(_context);

        public IScheduleRepository Schedules =>
            _schedules ??= new ScheduleRepository(_context);

        public IPatientRepository Patients =>
            _patients ??= new PatientRepository(_context);

        public IAppointmentRepository Appointments =>
            _appointments ??= new AppointmentRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
