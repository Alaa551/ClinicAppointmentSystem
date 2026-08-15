using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.DAL.Database.Entities;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.BLL.Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<AddEditScheduleRequest> _validator;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, IValidator<AddEditScheduleRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<ScheduleDto>> GetByDoctorAsync(int doctorId)
        {
            var schedules = await _unitOfWork.Repository<Schedule>()
                .Find(s => s.DoctorID == doctorId)
                .OrderBy(s => s.DayOfWeek)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto> AddAsync(AddEditScheduleRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            await EnsureDayNotAlreadyUsedAsync(request.DoctorID, request.DayOfWeek, excludeScheduleId: null);

            var schedule = _mapper.Map<Schedule>(request);
            await _unitOfWork.Repository<Schedule>().AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<ScheduleDto> EditAsync(AddEditScheduleRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var schedule = await _unitOfWork.Repository<Schedule>().GetByIdAsync(request.ScheduleID);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            await EnsureDayNotAlreadyUsedAsync(request.DoctorID, request.DayOfWeek, excludeScheduleId: request.ScheduleID);

            schedule.DayOfWeek = request.DayOfWeek;
            schedule.StartTime = request.StartTime;
            schedule.EndTime = request.EndTime;

            _unitOfWork.Repository<Schedule>().Update(schedule);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await _unitOfWork.Repository<Schedule>().GetByIdAsync(id);
            if (schedule == null)
                throw new KeyNotFoundException("Schedule not found.");

            _unitOfWork.Repository<Schedule>().Remove(schedule);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task EnsureDayNotAlreadyUsedAsync(int doctorId, DayOfWeek dayOfWeek, int? excludeScheduleId)
        {
            var existing = await _unitOfWork.Repository<Schedule>()
                .Find(s => s.DoctorID == doctorId && s.DayOfWeek == dayOfWeek)
                .FirstOrDefaultAsync();

            if (existing != null && existing.ScheduleID != excludeScheduleId)
            {
                throw new InvalidOperationException($"{dayOfWeek} already has hours set for this doctor. Edit or delete that row instead.");
            }
        }
    }
}
