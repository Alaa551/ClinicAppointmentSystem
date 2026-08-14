using ClinicAppointmentSystem.BLL.Mapping;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.BLL.Services.Implementations;
using ClinicAppointmentSystem.BLL.Validation;
using ClinicAppointmentSystem.DAL.Database.Data;
using ClinicAppointmentSystem.DAL.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // MVC
            builder.Services.AddControllersWithViews();

            // Database
            builder.Services.AddDbContext<ClinicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClinicDb")));

            // Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();

            // AutoMapper
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            // FluentValidation
            builder.Services.AddScoped<IValidator<ClinicAppointmentSystem.BLL.DTOs.AddEditDoctorRequest>, AddEditDoctorRequestValidator>();
            builder.Services.AddScoped<IValidator<ClinicAppointmentSystem.BLL.DTOs.AddEditPatientRequest>, AddEditPatientRequestValidator>();
            builder.Services.AddScoped<IValidator<ClinicAppointmentSystem.BLL.DTOs.CreateAppointmentRequest>, CreateAppointmentRequestValidator>();

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
