using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentsResultVisitor : ResultVisitorBase, IAppointmentsResultVisitor<Task<IActionResult>>
    {
        private readonly UserSession _userSession;
        private readonly ISessionCacheService _sessionCacheService;

        public AppointmentsResultVisitor(
            ISessionCacheService sessionCacheService,
            IErrorReferenceGenerator errorReferenceGenerator,
            UserSession userSession,
            Supplier supplier)
        : base (errorReferenceGenerator, supplier)
        {
            _sessionCacheService = sessionCacheService;
            _userSession = userSession;
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Appointments;

        public async Task<IActionResult> Visit(AppointmentsResult.Success result)
        {
            if (result.BookingReasonNecessity != null)
            {
                await _sessionCacheService.UpdateUserSession(_userSession);
            }

            return new OkObjectResult(result.Response);
        }

        public async Task<IActionResult> Visit(AppointmentsResult.BadRequest result)
        {
            return await Task.FromResult(BuildErrorResult(StatusCodes.Status400BadRequest));
        }

        public async Task<IActionResult> Visit(AppointmentsResult.BadGateway result)
        {
            return await Task.FromResult(BuildErrorResult(StatusCodes.Status502BadGateway));
        }

        public async Task<IActionResult> Visit(AppointmentsResult.InternalServerError result)
        {
            return await Task.FromResult(BuildErrorResult(StatusCodes.Status500InternalServerError));
        }

        public async Task<IActionResult> Visit(AppointmentsResult.Forbidden result)
        {
            return await Task.FromResult(BuildErrorResult(StatusCodes.Status403Forbidden));
        }
    }
}
