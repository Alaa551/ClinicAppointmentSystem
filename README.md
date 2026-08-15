# Clinic Appointment System

A clinic appointment booking system built with ASP.NET Core MVC, following a layered architecture (DAL / BLL / PL) with the Repository + Unit of Work pattern.

## Live Demo

The app is deployed and testable here: **http://clinicappointment.runasp.net/**

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
- **Pagination**: all list endpoints (Doctors, Patients, Appointments) use real server-side paging through DataTables, page size 10.
- **Controllers**: no shared base controller — each controller is self-contained and explicit.

## Entities

Doctor, Patient, Schedule, Appointment, Specialization. See [docs/erd.pdf](docs/erd.pdf) for the full diagram.

- **Doctor** — linked to a `Specialization` (lookup table, admin-extendable), has a weekly `Schedule`.
- **Patient** — address stored as separate `Street` / `City` / `ZipCode` fields; `Age` is derived from `BirthDate`, never stored.
- **Schedule** — one row per day of week per doctor (enforced by a unique index), defines working hours used to calculate free appointment slots.
- **Appointment** — booked in 30-minute slots against a doctor's schedule; status is `Booked`, `Cancelled`, or `Completed`.

## Setup

1. Set your connection string in `ClinicAppointmentSystem.PL/appsettings.Development.json` (local) under `ConnectionStrings:ClinicDb`.
2. Open Package Manager Console, set the default project to `ClinicAppointmentSystem.DAL`, and run:
   ```
   Update-Database -StartupProject ClinicAppointmentSystem.PL
   ```
   This applies all migrations, including the seeded `Specializations` lookup data.
3. Set `ClinicAppointmentSystem.PL` as the startup project and run.

