using ClinicAppointmentSystem.DAL.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointmentSystem.DAL.Database.Config
{
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.ToTable("Schedules");

            builder.HasKey(s => s.ScheduleID);

            builder.Property(s => s.DayOfWeek)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(s => new { s.DoctorID, s.DayOfWeek }).IsUnique();
        }
    }
}
