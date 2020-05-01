using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientSendMessageResultVisitor : ResultVisitorBase, IPatientSendMessageResultVisitor<IActionResult>
    {
        public PatientSendMessageResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, P9UserSession userSession)
            : base(errorReferenceGenerator, userSession){}

        protected override ErrorCategory ErrorCategory => ErrorCategory.PatientPracticeMessages;

        public IActionResult Visit(PostPatientMessageResult.Success result)
        {
            return new NoContentResult();
        }

        public IActionResult Visit(PostPatientMessageResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(PostPatientMessageResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(PostPatientMessageResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(PostPatientMessageResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}