using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
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

        // GET: /Doctors/GetAll  -> feeds the grid
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var doctors = await _doctorService.GetAllAsync();
                return (object)doctors;
            });
        }

        // GET: /Doctors/GetById?id=5  -> feeds the edit modal
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return await ExecuteAsync(async () =>
            {
                var doctor = await _doctorService.GetByIdAsync(id);
                return (object)doctor;
            });
        }

        // POST: /Doctors/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromBody] AddEditDoctorRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                var doctor = await _doctorService.AddAsync(request);
                return (object)doctor;
            });
        }

        // POST: /Doctors/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] AddEditDoctorRequest request)
        {
            return await ExecuteAsync(async () =>
            {
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
