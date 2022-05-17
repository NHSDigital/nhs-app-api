using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    public class SecondaryCareSummaryResultVisitor : ResultVisitorBase,
        ISecondaryCareSummaryResultVisitor<IActionResult>
    {
        public SecondaryCareSummaryResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, Supplier supplier) :
            base(errorReferenceGenerator, supplier)
        {
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.SecondaryCare;

        public IActionResult Visit(SecondaryCareSummaryResult.Success result)
            => new OkObjectResult(result.Response);

        public IActionResult Visit(SecondaryCareSummaryResult.BadGateway _)
            => BuildErrorResult(StatusCodes.Status502BadGateway);

        public IActionResult Visit(SecondaryCareSummaryResult.Timeout _)
            => BuildErrorResult(StatusCodes.Status504GatewayTimeout);

        public IActionResult Visit(SecondaryCareSummaryResult.FailedSecondaryCareMinimumAgeRequirement _)
            => BuildErrorResult(Constants.CustomHttpStatusCodes.Status470FailedSecondaryCareMinimumAgeRequirement);
    }
}