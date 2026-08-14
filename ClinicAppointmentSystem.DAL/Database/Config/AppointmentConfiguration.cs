using ClinicAppointmentSystem.DAL.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointmentSystem.DAL.Database.Config
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.AppointmentID);

            builder.Property(a => a.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20);

        }
    }
}
