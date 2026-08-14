using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.PL.Controllers
{
    public class AppointmentsController : BaseApiController
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly IPatientService _patientService;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IDoctorService doctorService,
            IPatientService patientService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _patientService = patientService;
        }

        // GET: /Appointments -> grid page with the booking modal
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Appointments/GetAll -> feeds the grid
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var appointments = await _appointmentService.GetAllAsync();
                return (object)appointments;
            });
        }

        // GET: /Appointments/SearchDoctors?term=ah -> Select2 remote source
        [HttpGet]
        public async Task<IActionResult> SearchDoctors(string term)
        {
            return await ExecuteAsync(async () =>
            {
                var doctors = await _doctorService.SearchActiveDoctorsAutoComplete(term);
                var results = doctors.Select(d => new { id = d.ID, text = d.Name });
                return (object)results;
            });
        }

        // GET: /Appointments/SearchPatients?term=sa -> Select2 remote source
        [HttpGet]
        public async Task<IActionResult> SearchPatients(string term)
        {
            return await ExecuteAsync(async () =>
            {
                var patients = await _patientService.SearchPatientsAutoComplete(term);
                var results = patients.Select(p => new { id = p.ID, text = p.Name });
                return (object)results;
            });
        }

        // GET: /Appointments/GetFreeSlots?doctorId=3&date=2026-08-20 -> feeds the slot dropdown
        [HttpGet]
        public async Task<IActionResult> GetFreeSlots(int doctorId, DateTime date)
        {
            return await ExecuteAsync(async () =>
            {
                var slots = await _appointmentService.GetFreeSlotsAsync(doctorId, date);
                var results = slots.Select(s => new
                {
                    value = s.StartTime.ToString(@"hh\:mm"),
                    label = $"{FormatTime(s.StartTime)} - {FormatTime(s.EndTime)}"
                });
                return (object)results;
            });
        }

        // POST: /Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
        {
            return await ExecuteAsync(async () =>
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(request);
                return (object)appointment;
            });
        }

        // POST: /Appointments/Cancel?id=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            return await ExecuteAsync(async () => await _appointmentService.CancelAsync(id));
        }

        // POST: /Appointments/Delete?id=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            return await ExecuteAsync(async () => await _appointmentService.DeleteAsync(id));
        }

        private static string FormatTime(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("h:mm tt");
        }
    }
}
