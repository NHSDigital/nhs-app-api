using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Appointments;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentsResultVisitor : IAppointmentsResultVisitor<Task<IActionResult>>
    {
        readonly ISessionCacheService _sessionCacheService;
        readonly UserSession _userSession;

        public AppointmentsResultVisitor(ISessionCacheService sessionCacheService, UserSession userSession)
        {
            _sessionCacheService = sessionCacheService;
            _userSession = userSession;
        }

        public async Task<IActionResult> Visit(AppointmentsResult.SuccessfullyRetrieved result)
        {
            if (result.BookingReasonNecessity != null)
            {
                await _sessionCacheService.UpdateUserSession(_userSession);
            }
            
            return new OkObjectResult(result.Response);
        }

        public async Task<IActionResult> Visit(AppointmentsResult.BadRequest result)
        {
            return await Task.FromResult(new BadRequestResult());
        }

        public async Task<IActionResult> Visit(AppointmentsResult.SupplierSystemUnavailable result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(AppointmentsResult.InternalServerError result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status500InternalServerError));
        }

        public async Task<IActionResult> Visit(AppointmentsResult.CannotViewAppointments result)
        {
            return await Task.FromResult(new StatusCodeResult(StatusCodes.Status403Forbidden));
        }
    }
}
