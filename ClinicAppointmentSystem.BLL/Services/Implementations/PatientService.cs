using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.BLL.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<AddEditPatientRequest> _validator;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AddEditPatientRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<PagedResult<PatientDto>> GetAllAsync(int pageNumber, int pageSize, string search)
        {
            var query = _unitOfWork.Repository<Patient>().GetAll();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.PhoneNumber.Contains(search) ||
                    p.Address.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var patients = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<PatientDto>
            {
                Items = _mapper.Map<IEnumerable<PatientDto>>(patients),
                TotalCount = totalCount
            };
        }

        public async Task<PatientDto> GetByIdAsync(int id)
        {
            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(id);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> AddAsync(AddEditPatientRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var patient = _mapper.Map<Patient>(request);
            await _unitOfWork.Repository<Patient>().AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> EditAsync(AddEditPatientRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(request.PatientID);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            patient.Name = request.Name;
            patient.BirthDate = request.BirthDate;
            patient.Gender = request.Gender;
            patient.PhoneNumber = request.PhoneNumber;
            patient.Address = request.Address;

            _unitOfWork.Repository<Patient>().Update(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(id);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            _unitOfWork.Repository<Patient>().Remove(patient);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<LookupModel>> SearchPatientsAutoComplete(string term)
        {
            var query = _unitOfWork.Repository<Patient>().GetAll();

            if (!string.IsNullOrWhiteSpace(term))
                query = query.Where(p => p.Name.Contains(term) || p.PhoneNumber.Contains(term));

            var patients = await query
                .OrderBy(p => p.Name)
                .Take(10)
                .ToListAsync();

            return patients.Select(p => new LookupModel { ID = p.PatientID, Name = p.Name });
        }
    }
}
