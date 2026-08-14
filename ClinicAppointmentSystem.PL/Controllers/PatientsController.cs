using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
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

        // POST: /Patients/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromBody] AddEditPatientRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                var patient = await _patientService.AddAsync(request);
                return (object)patient;
            });
        }

        // POST: /Patients/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] AddEditPatientRequest request)
        {
            return await ExecuteAsync(async () =>
            {
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
