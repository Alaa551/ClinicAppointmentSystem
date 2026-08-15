using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicAppointmentSystem.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LatestSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_DoctorID",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorID",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Doctors");

            migrationBuilder.AddColumn<int>(
                name: "SpecializationID",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    SpecializationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.SpecializationID);
                });

            migrationBuilder.InsertData(
                table: "Specializations",
                columns: new[] { "SpecializationID", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "General Practice" },
                    { 2, true, "Cardiology" },
                    { 3, true, "Dermatology" },
                    { 4, true, "Pediatrics" },
                    { 5, true, "Neurology" },
                    { 6, true, "Orthopedics" },
                    { 7, true, "Dentistry" },
                    { 8, true, "Ophthalmology" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DoctorID_DayOfWeek",
                table: "Schedules",
                columns: new[] { "DoctorID", "DayOfWeek" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_IsActive",
                table: "Doctors",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_SpecializationID",
                table: "Doctors",
                column: "SpecializationID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorID_AppointmentDate",
                table: "Appointments",
                columns: new[] { "DoctorID", "AppointmentDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specializations_SpecializationID",
                table: "Doctors",
                column: "SpecializationID",
                principalTable: "Specializations",
                principalColumn: "SpecializationID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specializations_SpecializationID",
                table: "Doctors");

            migrationBuilder.DropTable(
                name: "Specializations");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_DoctorID_DayOfWeek",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_IsActive",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_SpecializationID",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorID_AppointmentDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "SpecializationID",
                table: "Doctors");

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Doctors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DoctorID",
                table: "Schedules",
                column: "DoctorID");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorID",
                table: "Appointments",
                column: "DoctorID");
        }
    }
}
