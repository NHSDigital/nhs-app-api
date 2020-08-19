using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    internal class GetPrescriptionsResultVisitor : ResultVisitorBase, IGetPrescriptionsResultVisitor<IActionResult>
    {
        public GetPrescriptionsResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, Supplier supplier)
            : base(errorReferenceGenerator, supplier)
        {
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.Prescriptions;

        public IActionResult Visit(GetPrescriptionsResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetPrescriptionsResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetPrescriptionsResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(GetPrescriptionsResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetPrescriptionsResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}