using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.DAL.Database.Entities;

namespace ClinicAppointmentSystem.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.Name));

            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.SpecializationName, opt => opt.MapFrom(src => src.Specialization.Name));
            CreateMap<AddEditDoctorRequest, Doctor>();

            CreateMap<Patient, PatientDto>();
            CreateMap<AddEditPatientRequest, Patient>();

            CreateMap<Schedule, ScheduleDto>();
            CreateMap<AddEditScheduleRequest, Schedule>();
        }
    }
}
