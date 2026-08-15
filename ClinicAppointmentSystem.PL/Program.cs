using ClinicAppointmentSystem.BLL.Mapping;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.BLL.Services.Implementations;
using ClinicAppointmentSystem.BLL.Validation;
using ClinicAppointmentSystem.DAL.Database.Data;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace ClinicAppointmentSystem.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            builder.Services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClinicDb")));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<ISpecializationService, SpecializationService>();

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            builder.Services.AddScoped<IValidator<ClinicAppointmentSystem.BLL.DTOs.AddEditDoctorRequest>, AddEditDoctorRequestValidator>();
            builder.Services.AddScoped<IValidator<ClinicAppointmentSystem.BLL.DTOs.AddEditPatientRequest>, AddEditPatientRequestValidator>();
            builder.Services.AddScoped<IValidator<ClinicAppointmentSystem.BLL.DTOs.AddEditAppointmentRequest>, AddEditAppointmentRequestValidator>();
            builder.Services.AddScoped<IValidator<ClinicAppointmentSystem.BLL.DTOs.AddEditScheduleRequest>, AddEditScheduleRequestValidator>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Appointments}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
