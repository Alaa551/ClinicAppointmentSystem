using AutoMapper;
using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.DAL.Database.Entities;

namespace ClinicAppointmentSystem.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---- Appointment ----
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.Name));

            // ---- Doctor ----
            CreateMap<Doctor, DoctorDto>();
            CreateMap<AddEditDoctorRequest, Doctor>();

            // ---- Patient ----
            CreateMap<Patient, PatientDto>();
            CreateMap<AddEditPatientRequest, Patient>();
        }
    }
}
