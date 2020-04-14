using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentBookResultVisitor : ResultVisitorBase, IAppointmentBookResultVisitor<IActionResult>
    {
        public AppointmentBookResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, P9UserSession userSession) 
            : base(errorReferenceGenerator, userSession)
        {
        }
        
        protected override ErrorCategory ErrorCategory => ErrorCategory.Appointments;
        
        public IActionResult Visit(AppointmentBookResult.Success result)
        {
            return new CreatedResult(string.Empty, null);
        }

        public IActionResult Visit(AppointmentBookResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(AppointmentBookResult.SlotNotAvailable result)
        {
            return BuildErrorResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(AppointmentBookResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AppointmentBookResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(AppointmentBookResult.AppointmentLimitReached result)
        {
            return BuildErrorResult(CustomHttpStatusCodes.Status460LimitReached);
        }

        public IActionResult Visit(AppointmentBookResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}
