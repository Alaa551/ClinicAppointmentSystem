using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.Enums;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;

namespace ClinicAppointmentSystem.BLL.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateAppointmentRequest> _validator;
        private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);

        public AppointmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<CreateAppointmentRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek;
            var schedule = await _unitOfWork.Schedules.GetByDoctorAndDayAsync(doctorId, dayOfWeek);

            if (schedule == null)
            {
                // Doctor does not work on this day (e.g. Friday) -> no slots
                return Enumerable.Empty<FreeSlotDto>();
            }

            var candidateSlots = new List<FreeSlotDto>();
            var cursor = schedule.StartTime;

            while (cursor + SlotDuration <= schedule.EndTime)
            {
                candidateSlots.Add(new FreeSlotDto
                {
                    StartTime = cursor,
                    EndTime = cursor + SlotDuration
                });
                cursor += SlotDuration;
            }

            var bookedAppointments = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, date);
            var bookedStartTimes = bookedAppointments
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.StartTime)
                .ToHashSet();

            return candidateSlots
                .Where(s => !bookedStartTimes.Contains(s.StartTime))
                .ToList();
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Doctor must exist and be active
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorID);
            if (doctor == null || !doctor.IsActive)
                throw new InvalidOperationException("The selected doctor is not available.");

            var patient = await _unitOfWork.Patients.GetByIdAsync(request.PatientID);
            if (patient == null)
                throw new InvalidOperationException("Selected patient was not found.");

            // Re-check the slot is still free (race-condition safety —
            // another secretary may have booked it between page load and submit)
            var freeSlots = await GetFreeSlotsAsync(request.DoctorID, request.AppointmentDate);
            var isStillFree = freeSlots.Any(s => s.StartTime == request.StartTime);
            if (!isStillFree)
                throw new InvalidOperationException("The selected time slot is no longer available.");

            var appointment = new Appointment
            {
                PatientID = request.PatientID,
                DoctorID = request.DoctorID,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                EndTime = request.StartTime + SlotDuration,
                Status = AppointmentStatus.Booked
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task CancelAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            appointment.Status = AppointmentStatus.Cancelled;
            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            _unitOfWork.Appointments.Remove(appointment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}