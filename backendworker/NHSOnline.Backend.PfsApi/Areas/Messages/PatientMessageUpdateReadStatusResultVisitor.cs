using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageUpdateReadStatusResultVisitor : ResultVisitorBase, IPatientMessageUpdateReadStatusResultVisitor<IActionResult>
    {
        public PatientMessageUpdateReadStatusResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, Supplier supplier)
            : base(errorReferenceGenerator, supplier){}

        protected override ErrorCategory ErrorCategory => ErrorCategory.PatientPracticeMessages;

        public IActionResult Visit(PutPatientMessageReadStatusResult.Success result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(PutPatientMessageReadStatusResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(PutPatientMessageReadStatusResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(PutPatientMessageReadStatusResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(PutPatientMessageReadStatusResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}