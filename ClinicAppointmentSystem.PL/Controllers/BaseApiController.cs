using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.PL.Controllers
{
    // Shared JSON response shape + exception handling for AJAX endpoints.
    public abstract class BaseApiController : Controller
    {
        protected IActionResult Ok(object data)
        {
            return Json(new { success = true, data });
        }

        protected IActionResult Fail(string message)
        {
            return Json(new { success = false, message });
        }

        protected string GetFirstModelError()
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

        protected async Task<IActionResult> ExecuteAsync(Func<Task<object>> action)
        {
            try
            {
                var result = await action();
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                var message = string.Join(" ", ex.Errors.Select(e => e.ErrorMessage));
                return Fail(message);
            }
            catch (KeyNotFoundException ex)
            {
                return Fail(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Fail(ex.Message);
            }
        }

        protected async Task<IActionResult> ExecuteAsync(Func<Task> action)
        {
            return await ExecuteAsync(async () =>
            {
                await action();
                return (object)null;
            });
        }
    }
}
