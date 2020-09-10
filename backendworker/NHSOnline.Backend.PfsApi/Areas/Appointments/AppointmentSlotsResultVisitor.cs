﻿using Microsoft.AspNetCore.Http;
 using Microsoft.AspNetCore.Mvc;
 using NHSOnline.Backend.GpSystems.Appointments;
 using NHSOnline.Backend.Support;

 namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentSlotsResultVisitor : ResultVisitorBase, IAppointmentSlotsResultVisitor<IActionResult>
    {
        public AppointmentSlotsResultVisitor(
            IErrorReferenceGenerator errorReferenceGenerator,
            Supplier supplier) :
            base(errorReferenceGenerator, supplier)
        {
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Appointments;

        public IActionResult Visit(AppointmentSlotsResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(AppointmentSlotsResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(AppointmentSlotsResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(AppointmentSlotsResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }
    }
}
