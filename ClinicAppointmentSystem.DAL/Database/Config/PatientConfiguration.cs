using ClinicAppointmentSystem.DAL.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointmentSystem.DAL.Database.Config
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.PatientID);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Gender)
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(p => p.Street)
                .HasMaxLength(150);

            builder.Property(p => p.City)
                .HasMaxLength(100);

            builder.Property(p => p.ZipCode)
                .HasMaxLength(20);


            builder.HasMany(p => p.Appointments)
                .WithOne(a => a.Patient)
                .HasForeignKey(a => a.PatientID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
