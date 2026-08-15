using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.PL.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.PL.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var result = await _patientService.GetAllAsync(pageNumber, pageSize, search);
            return Json(new
            {
                totalCount = result.TotalCount,
                items = result.Items
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return Json(new
            {
                success = true,
                data = patient
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PatientFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditPatientRequest
                {
                    PatientID = 0,
                    Name = model.Name,
                    BirthDate = model.BirthDate.Value,
                    Gender = model.Gender.Value,
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    City = model.City,
                    ZipCode = model.ZipCode
                };
                var patient = await _patientService.AddAsync(request);
                return Json(new
                {
                    success = true,
                    data = patient
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
        public async Task<IActionResult> Edit(PatientFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditPatientRequest
                {
                    PatientID = model.PatientID,
                    Name = model.Name,
                    BirthDate = model.BirthDate.Value,
                    Gender = model.Gender.Value,
                    PhoneNumber = model.PhoneNumber,
                    Street = model.Street,
                    City = model.City,
                    ZipCode = model.ZipCode
                };
                var patient = await _patientService.EditAsync(request);
                return Json(new
                {
                    success = true,
                    data = patient
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _patientService.DeleteAsync(id);
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
    }
}