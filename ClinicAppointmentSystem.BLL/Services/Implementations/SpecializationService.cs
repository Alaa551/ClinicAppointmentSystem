using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.BLL.Services.Implementations
{
    public class SpecializationService : ISpecializationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecializationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LookupModel>> GetActiveAsync()
        {
            var specializations = await _unitOfWork.Repository<Specialization>()
                .Find(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return specializations.Select(s => new LookupModel { ID = s.SpecializationID, Name = s.Name });
        }
    }
}
