using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.PL.Controllers
{
    public class DoctorsController : BaseApiController
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: /Doctors
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Doctors/GetAll -> feeds the grid
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var doctors = await _doctorService.GetAllAsync();
                return (object)doctors;
            });
        }

        // GET: /Doctors/GetById?id=5 -> feeds the edit modal
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var doctor = await _doctorService.GetByIdAsync(id);
                return (object)doctor;
            });
        }

        // POST: /Doctors/Add  (form-encoded, bound via DataAnnotations on DoctorFormViewModel)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(DoctorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Fail(GetFirstModelError());

            return await ExecuteAsync(async () =>
            {
                var request = new AddEditDoctorRequest
                {
                    DoctorID = 0,
                    Name = model.Name,
                    Specialization = model.Specialization,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    IsActive = model.IsActive
                };
                var doctor = await _doctorService.AddAsync(request);
                return (object)doctor;
            });
        }

        // POST: /Doctors/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DoctorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Fail(GetFirstModelError());

            return await ExecuteAsync(async () =>
            {
                var request = new AddEditDoctorRequest
                {
                    DoctorID = model.DoctorID,
                    Name = model.Name,
                    Specialization = model.Specialization,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    IsActive = model.IsActive
                };
                var doctor = await _doctorService.EditAsync(request);
                return (object)doctor;
            });
        }

        // POST: /Doctors/Delete?id=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteAsync(async () => await _doctorService.DeleteAsync(id));
        }
    }
}
