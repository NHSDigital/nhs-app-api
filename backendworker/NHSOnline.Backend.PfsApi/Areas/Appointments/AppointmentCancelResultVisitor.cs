using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentCancelResultVisitor : ResultVisitorBase, IAppointmentCancelResultVisitor<IActionResult>
    {
        public AppointmentCancelResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, P9UserSession userSession) 
            : base(errorReferenceGenerator, userSession)
        {
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Appointments;
        
        public IActionResult Visit(AppointmentCancelResult.Success result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(AppointmentCancelResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(AppointmentCancelResult.AppointmentNotCancellable result)
        {
            return BuildErrorResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(AppointmentCancelResult.TooLateToCancel result)
        {
            return BuildErrorResult(Constants.CustomHttpStatusCodes.Status461TooLate);
        }

        public IActionResult Visit(AppointmentCancelResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(AppointmentCancelResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AppointmentCancelResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}
