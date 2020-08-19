using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    internal class OrderPrescriptionResultVisitor : ResultVisitorBase, IOrderPrescriptionResultVisitor<IActionResult>
    {
        public OrderPrescriptionResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, P9UserSession userSession)
            : base(errorReferenceGenerator, userSession)
        {
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Prescriptions;

        public IActionResult Visit(OrderPrescriptionResult.Success result)
        {
            return new CreatedResult(string.Empty, null);
        }

        public IActionResult Visit(OrderPrescriptionResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(OrderPrescriptionResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(OrderPrescriptionResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(OrderPrescriptionResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }

        public IActionResult Visit(OrderPrescriptionResult.CannotReorderPrescription result)
        {
            return BuildErrorResult(StatusCodes.Status409Conflict);
        }

        public IActionResult Visit(OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            return BuildErrorResult(Constants.CustomHttpStatusCodes.Status466MedicationAlreadyOrderedWithinLast30Days);
        }

        public IActionResult Visit(OrderPrescriptionResult.PartialSuccess result)
        {
            return new OkObjectResult(result.Response)
            {
                StatusCode = StatusCodes.Status202Accepted,
            };
        }
    }
}
