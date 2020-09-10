using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageRecipientsResultVisitor : ResultVisitorBase, IPatientMessageRecipientsResultVisitor<IActionResult>
    {
        public PatientMessageRecipientsResultVisitor(IErrorReferenceGenerator errorReferenceGenerator,
            Supplier supplier) : base(errorReferenceGenerator, supplier){}

        protected override ErrorCategory ErrorCategory => ErrorCategory.PatientPracticeMessages;

        public IActionResult Visit(GetPatientMessageRecipientsResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetPatientMessageRecipientsResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetPatientMessageRecipientsResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(GetPatientMessageRecipientsResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetPatientMessageRecipientsResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}