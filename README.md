# Clinic Appointment System

A clinic appointment booking system built with ASP.NET Core MVC, following a layered architecture (DAL / BLL / PL) with the Repository + Unit of Work pattern.

## Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (SQL Server)
- AutoMapper
- FluentValidation
- Bootstrap 5, DataTables (server-side), Select2, Flatpickr, jQuery Validation Unobtrusive, SweetAlert2, intl-tel-input

## Project Structure

```
ClinicAppointmentSystem.DAL/    Entities, EF Core configurations, migrations, generic repository, unit of work
ClinicAppointmentSystem.BLL/    DTOs, validators, services (business logic, pagination, mapping)
ClinicAppointmentSystem.PL/     Controllers, views, view models, static assets
```

## Architecture Notes

- **Repository**: a single generic `IGenericRepository<T>` exposes `IQueryable<T>` — services handle their own `.Include()`, filtering, and paging.
- **Unit of Work**: `IUnitOfWork.Repository<T>()` returns a cached generic repository per entity type; no per-entity repository classes.
- **Pagination**: all list endpoints (Doctors, Patients, Appointments) use real server-side paging through DataTables' standard AJAX contract (`draw`, `start`, `length`, `search[value]`), page size 10.

## Entities

Doctor, Patient, Schedule, Appointment, Specialization. See `docs/erd.pdf` for the full diagram.

## Setup

1. Set your connection string in `ClinicAppointmentSystem.PL/appsettings.json` under `ConnectionStrings:ClinicDb`.
2. Open Package Manager Console, set the default project to `ClinicAppointmentSystem.DAL`, and run:
   ```
   Add-Migration LatestSchema -StartupProject ClinicAppointmentSystem.PL
   Update-Database -StartupProject ClinicAppointmentSystem.PL
   ```
   (A new migration is required — this update added the `Specialization` table, the `Doctor.SpecializationID` foreign key, and composite indexes on `Schedule` and `Appointment`.)
3. Set `ClinicAppointmentSystem.PL` as the startup project and run.

## Notes

- Free/busy time slots are calculated from each doctor's weekly `Schedule` in 30-minute increments.
- A doctor can only have one `Schedule` row per day of week (enforced by a unique index and at the service layer).
- Appointments can be booked, edited, cancelled, or deleted; only appointments with status `Booked` can be edited or cancelled.
