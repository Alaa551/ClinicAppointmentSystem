using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.PL.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.PL.Controllers
{
    public class AppointmentsController : Controller
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

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var result = await _appointmentService.GetAllAsync(pageNumber, pageSize, search);
            return Json(new
            {
                totalCount = result.TotalCount,
                items = result.Items
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            return Json(new
            {
                success = true,
                data = appointment
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchDoctors(string term)
        {
            var doctors = await _doctorService.SearchActiveDoctorsAutoComplete(term);
            var results = doctors.Select(d => new { id = d.ID, text = d.Name });
            return Json(new
            {
                success = true,
                data = results
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchPatients(string term)
        {
            var patients = await _patientService.SearchPatientsAutoComplete(term);
            var results = patients.Select(p => new { id = p.ID, text = p.Name });
            return Json(new
            {
                success = true,
                data = results
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetFreeSlots(int doctorId, DateTime date, int? excludeAppointmentId)
        {
            var slots = await _appointmentService.GetFreeSlotsAsync(doctorId, date, excludeAppointmentId);
            var results = slots.Select(s => new
            {
                value = s.StartTime.ToString(@"hh\:mm"),
                label = $"{FormatTime(s.StartTime)} - {FormatTime(s.EndTime)}"
            });
            return Json(new
            {
                success = true,
                data = results
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditAppointmentRequest
                {
                    AppointmentID = 0,
                    DoctorID = model.DoctorID,
                    PatientID = model.PatientID,
                    AppointmentDate = model.AppointmentDate.Value,
                    StartTime = TimeSpan.Parse(model.StartTime)
                };
                var appointment = await _appointmentService.CreateAppointmentAsync(request);
                return Json(new
                {
                    success = true,
                    data = appointment
                });
            }
            catch (ValidationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage))
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditAppointmentRequest
                {
                    AppointmentID = model.AppointmentID,
                    DoctorID = model.DoctorID,
                    PatientID = model.PatientID,
                    AppointmentDate = model.AppointmentDate.Value,
                    StartTime = TimeSpan.Parse(model.StartTime)
                };
                var appointment = await _appointmentService.EditAppointmentAsync(request);
                return Json(new
                {
                    success = true,
                    data = appointment
                });
            }
            catch (ValidationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage))
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _appointmentService.CancelAsync(id);
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _appointmentService.DeleteAsync(id);
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private string GetFirstModelError()
        {
            foreach (var entry in ModelState.Values)
            {
                foreach (var error in entry.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                        return error.ErrorMessage;
                }
            }
            return "Please check the form for errors.";
        }

        private static string FormatTime(TimeSpan time)
        {
            return DateTime.Today.Add(time).ToString("h:mm tt");
        }
    }
}