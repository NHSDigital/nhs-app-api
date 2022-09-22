using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.SecondaryCare;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.SecondaryCare
{
    public class SecondaryCareWaitTimesResultVisitor : ResultVisitorBase,
        ISecondaryCareWaitTimesResultVisitor<IActionResult>
    {
        public SecondaryCareWaitTimesResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, Supplier supplier) :
            base(errorReferenceGenerator, supplier)
        {
        }

        protected override ErrorCategory ErrorCategory => ErrorCategory.SecondaryCare;

        public IActionResult Visit(SecondaryCareWaitTimesResult.Success result)
            => new OkObjectResult(result.Response);

        public IActionResult Visit(SecondaryCareWaitTimesResult.BadGateway _)
            => BuildErrorResult(StatusCodes.Status502BadGateway);

        public IActionResult Visit(SecondaryCareWaitTimesResult.Timeout _)
            => BuildErrorResult(StatusCodes.Status504GatewayTimeout);

        public IActionResult Visit(SecondaryCareWaitTimesResult.NotEnabled _)
            => new NotFoundResult();
    }
}