using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;

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

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _unitOfWork.Patients.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto> GetByIdAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> AddAsync(AddEditPatientRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var patient = _mapper.Map<Patient>(request);
            await _unitOfWork.Patients.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto> EditAsync(AddEditPatientRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientID);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            patient.Name = request.Name;
            patient.BirthDate = request.BirthDate;
            patient.Gender = request.Gender;
            patient.PhoneNumber = request.PhoneNumber;
            patient.Address = request.Address;

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id);
            if (patient == null)
                throw new KeyNotFoundException("Patient not found.");

            _unitOfWork.Patients.Remove(patient);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<LookupModel>> SearchPatientsAutoComplete(string term)
        {
            var patients = await _unitOfWork.Patients.SearchPatientsAutoComplete(term);

            return patients.Select(p => new LookupModel
            {
                ID = p.PatientID,
                Name = p.Name
            });
        }
    }
}