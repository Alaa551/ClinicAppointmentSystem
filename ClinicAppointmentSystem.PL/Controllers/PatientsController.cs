using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.PL.Controllers
{
    public class PatientsController : BaseApiController
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: /Patients
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Patients/GetAll -> feeds the grid
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var patients = await _patientService.GetAllAsync();
                return (object)patients;
            });
        }

        // GET: /Patients/GetById?id=5 -> feeds the edit modal
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var patient = await _patientService.GetByIdAsync(id);
                return (object)patient;
            });
        }

        // POST: /Patients/Add  (form-encoded, bound via DataAnnotations on PatientFormViewModel)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PatientFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Fail(GetFirstModelError());

            return await ExecuteAsync(async () =>
            {
                var request = new AddEditPatientRequest
                {
                    PatientID = 0,
                    Name = model.Name,
                    BirthDate = model.BirthDate.Value,
                    Gender = model.Gender.Value,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };
                var patient = await _patientService.AddAsync(request);
                return (object)patient;
            });
        }

        // POST: /Patients/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Fail(GetFirstModelError());

            return await ExecuteAsync(async () =>
            {
                var request = new AddEditPatientRequest
                {
                    PatientID = model.PatientID,
                    Name = model.Name,
                    BirthDate = model.BirthDate.Value,
                    Gender = model.Gender.Value,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };
                var patient = await _patientService.EditAsync(request);
                return (object)patient;
            });
        }

        // POST: /Patients/Delete?id=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteAsync(async () => await _patientService.DeleteAsync(id));
        }
    }
}
