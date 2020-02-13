using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageDeleteResultVisitor : ResultVisitorBase, IPatientMessageDeleteResultVisitor<IActionResult>
    {
        public PatientMessageDeleteResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, UserSession userSession)
            : base(errorReferenceGenerator, userSession){}

        protected override ErrorCategory ErrorCategory => ErrorCategory.PatientPracticeMessages;

        public IActionResult Visit(DeletePatientMessageResult.Success result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(DeletePatientMessageResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(DeletePatientMessageResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(DeletePatientMessageResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(DeletePatientMessageResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}