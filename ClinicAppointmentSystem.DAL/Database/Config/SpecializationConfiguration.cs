using ClinicAppointmentSystem.DAL.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicAppointmentSystem.DAL.Database.Config
{
    public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.ToTable("Specializations");

            builder.HasKey(s => s.SpecializationID);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasData(
                new Specialization { SpecializationID = 1, Name = "General Practice", IsActive = true },
                new Specialization { SpecializationID = 2, Name = "Cardiology", IsActive = true },
                new Specialization { SpecializationID = 3, Name = "Dermatology", IsActive = true },
                new Specialization { SpecializationID = 4, Name = "Pediatrics", IsActive = true },
                new Specialization { SpecializationID = 5, Name = "Neurology", IsActive = true },
                new Specialization { SpecializationID = 6, Name = "Orthopedics", IsActive = true },
                new Specialization { SpecializationID = 7, Name = "Dentistry", IsActive = true },
                new Specialization { SpecializationID = 8, Name = "Ophthalmology", IsActive = true }
            );
        }
    }
}
