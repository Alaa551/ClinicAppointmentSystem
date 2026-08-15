using ClinicAppointmentSystem.BLL.DTOs;
using ClinicAppointmentSystem.BLL.Services.Abstraction;
using ClinicAppointmentSystem.PL.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.PL.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _doctorService;
        private readonly IScheduleService _scheduleService;
        private readonly ISpecializationService _specializationService;

        public DoctorsController(
            IDoctorService doctorService,
            IScheduleService scheduleService,
            ISpecializationService specializationService)
        {
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _specializationService = specializationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            ViewData["DoctorID"] = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string search = "")
        {
            var result = await _doctorService.GetAllAsync(pageNumber, pageSize, search);
            return Json(new
            {
                totalCount = result.TotalCount,
                items = result.Items
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecializations()
        {
            var specializations = await _specializationService.GetActiveAsync();
            return Json(new
            {
                success = true,
                data = specializations
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            return Json(new
            {
                success = true,
                data = doctor
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(DoctorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditDoctorRequest
                {
                    DoctorID = 0,
                    Name = model.Name,
                    SpecializationID = model.SpecializationID,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    IsActive = model.IsActive
                };
                var doctor = await _doctorService.AddAsync(request);
                return Json(new
                {
                    success = true,
                    data = doctor
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
        public async Task<IActionResult> Edit(DoctorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditDoctorRequest
                {
                    DoctorID = model.DoctorID,
                    Name = model.Name,
                    SpecializationID = model.SpecializationID,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    IsActive = model.IsActive
                };
                var doctor = await _doctorService.EditAsync(request);
                return Json(new
                {
                    success = true,
                    data = doctor
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
                await _doctorService.DeleteAsync(id);
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

        [HttpGet]
        public async Task<IActionResult> GetSchedules(int doctorId)
        {
            var schedules = await _scheduleService.GetByDoctorAsync(doctorId);
            return Json(new
            {
                success = true,
                data = schedules
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSchedule(ScheduleFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditScheduleRequest
                {
                    ScheduleID = 0,
                    DoctorID = model.DoctorID,
                    DayOfWeek = model.DayOfWeek.Value,
                    StartTime = model.StartTime.Value,
                    EndTime = model.EndTime.Value
                };
                var schedule = await _scheduleService.AddAsync(request);
                return Json(new
                {
                    success = true,
                    data = schedule
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
        public async Task<IActionResult> EditSchedule(ScheduleFormViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = GetFirstModelError()
                });

            try
            {
                var request = new AddEditScheduleRequest
                {
                    ScheduleID = model.ScheduleID.GetValueOrDefault(),
                    DoctorID = model.DoctorID,
                    DayOfWeek = model.DayOfWeek.GetValueOrDefault(),
                    StartTime = model.StartTime.GetValueOrDefault(),
                    EndTime = model.EndTime.GetValueOrDefault()
                };
                var schedule = await _scheduleService.EditAsync(request);
                return Json(new
                {
                    success = true,
                    data = schedule
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
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                await _scheduleService.DeleteAsync(id);
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