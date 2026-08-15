using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.BLL.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<AddEditDoctorRequest> _validator;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AddEditDoctorRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<PagedResult<DoctorDto>> GetAllAsync(int pageNumber, int pageSize, string search)
        {
            IQueryable<Doctor> query = _unitOfWork.Repository<Doctor>()
                                                  .GetAll()
                                                  .Include(d => d.Specialization);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.Name.Contains(search) ||
                    d.Email.Contains(search) ||
                    d.PhoneNumber.Contains(search) ||
                    d.Specialization.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var doctors = await query
                .OrderBy(d => d.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<DoctorDto>
            {
                Items = _mapper.Map<IEnumerable<DoctorDto>>(doctors),
                TotalCount = totalCount
            };
        }

        public async Task<DoctorDto> GetByIdAsync(int id)
        {
            var doctor = await _unitOfWork.Repository<Doctor>()
                .Find(d => d.DoctorID == id)
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> AddAsync(AddEditDoctorRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var doctor = _mapper.Map<Doctor>(request);
            await _unitOfWork.Repository<Doctor>().AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(doctor.DoctorID);
        }

        public async Task<DoctorDto> EditAsync(AddEditDoctorRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(request.DoctorID);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            doctor.Name = request.Name;
            doctor.SpecializationID = request.SpecializationID;
            doctor.PhoneNumber = request.PhoneNumber;
            doctor.Email = request.Email;
            doctor.IsActive = request.IsActive;

            _unitOfWork.Repository<Doctor>().Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(doctor.DoctorID);
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            _unitOfWork.Repository<Doctor>().Remove(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<LookupModel>> SearchActiveDoctorsAutoComplete(string term, int maxResults = 10)
        {
            var query = _unitOfWork.Repository<Doctor>().Find(d => d.IsActive);

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(d => d.Name.Contains(term));

            var doctors = await query
                .OrderBy(d => d.Name)
                .Take(maxResults)
                .ToListAsync();

            return doctors.Select(d => new LookupModel { ID = d.DoctorID, Name = d.Name });
        }
    }
}
