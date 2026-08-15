using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.Enums;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.BLL.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<AddEditAppointmentRequest> _validator;
        private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);

        public AppointmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<AddEditAppointmentRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<PagedResult<AppointmentDto>> GetAllAsync(int pageNumber, int pageSize, string search)
        {
            IQueryable<Appointment> query = _unitOfWork.Repository<Appointment>()
                                                        .GetAll()
                                                        .Include(a => a.Doctor)
                                                        .Include(a => a.Patient);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.Doctor.Name.Contains(search) ||
                    a.Patient.Name.Contains(search));
            }

            var totalCount = await query.CountAsync();

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<AppointmentDto>
            {
                Items = _mapper.Map<IEnumerable<AppointmentDto>>(appointments),
                TotalCount = totalCount
            };
        }

        public async Task<AppointmentDto> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.Repository<Appointment>()
                .Find(a => a.AppointmentID == id)
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync();

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, DateTime date, int? excludeAppointmentId = null)
        {
            var dayOfWeek = date.DayOfWeek;

            var schedule = await _unitOfWork.Repository<Schedule>()
                .Find(s => s.DoctorID == doctorId && s.DayOfWeek == dayOfWeek)
                .FirstOrDefaultAsync();

            if (schedule == null)
                return Enumerable.Empty<FreeSlotDto>();

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

            var bookedAppointments = await _unitOfWork.Repository<Appointment>()
                .Find(a => a.DoctorID == doctorId
                    && a.AppointmentDate.Date == date.Date
                    && a.Status != AppointmentStatus.Cancelled
                    && (excludeAppointmentId == null || a.AppointmentID != excludeAppointmentId))
                .ToListAsync();

            var bookedStartTimes = bookedAppointments.Select(a => a.StartTime).ToHashSet();

            return candidateSlots
                .Where(s => !bookedStartTimes.Contains(s.StartTime))
                .ToList();
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(AddEditAppointmentRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(request.DoctorID);
            if (doctor == null || !doctor.IsActive)
                throw new InvalidOperationException("The selected doctor is not available.");

            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(request.PatientID);
            if (patient == null)
                throw new InvalidOperationException("Selected patient was not found.");

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

            await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(appointment.AppointmentID);
        }

        public async Task<AppointmentDto> EditAppointmentAsync(AddEditAppointmentRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(request.AppointmentID);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Booked)
                throw new InvalidOperationException("Only booked appointments can be edited.");

            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(request.DoctorID);
            if (doctor == null || !doctor.IsActive)
                throw new InvalidOperationException("The selected doctor is not available.");

            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(request.PatientID);
            if (patient == null)
                throw new InvalidOperationException("Selected patient was not found.");

            var freeSlots = await GetFreeSlotsAsync(request.DoctorID, request.AppointmentDate, excludeAppointmentId: appointment.AppointmentID);
            var isStillFree = freeSlots.Any(s => s.StartTime == request.StartTime);
            if (!isStillFree)
                throw new InvalidOperationException("The selected time slot is no longer available.");

            appointment.DoctorID = request.DoctorID;
            appointment.PatientID = request.PatientID;
            appointment.AppointmentDate = request.AppointmentDate;
            appointment.StartTime = request.StartTime;
            appointment.EndTime = request.StartTime + SlotDuration;

            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(appointment.AppointmentID);
        }

        public async Task CancelAsync(int id)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            appointment.Status = AppointmentStatus.Cancelled;
            _unitOfWork.Repository<Appointment>().Update(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            _unitOfWork.Repository<Appointment>().Remove(appointment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
