using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;

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

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var doctors = await _unitOfWork.Doctors.GetAllAsync();
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> GetByIdAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> AddAsync(AddEditDoctorRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var doctor = _mapper.Map<Doctor>(request);
            await _unitOfWork.Doctors.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task<DoctorDto> EditAsync(AddEditDoctorRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorID);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            doctor.Name = request.Name;
            doctor.Specialization = request.Specialization;
            doctor.PhoneNumber = request.PhoneNumber;
            doctor.Email = request.Email;
            doctor.IsActive = request.IsActive;

            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DoctorDto>(doctor);
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null)
                throw new KeyNotFoundException("Doctor not found.");

            _unitOfWork.Doctors.Remove(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<LookupModel>> SearchActiveDoctorsAutoComplete(string term)
        {
            var doctors = await _unitOfWork.Doctors.SearchActiveDoctorsAutoComplete(term);

            return doctors.Select(d => new LookupModel
            {
                ID = d.DoctorID,
                Name = d.Name
            });
        }
    }
}