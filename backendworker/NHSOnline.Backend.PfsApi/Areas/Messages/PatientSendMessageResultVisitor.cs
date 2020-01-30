using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientSendMessageResultVisitor : ResultVisitorBase, IPatientSendMessageResultVisitor<IActionResult>
    {
        public PatientSendMessageResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, UserSession userSession) 
            : base(errorReferenceGenerator, userSession){}
        
        protected override ErrorCategory ErrorCategory => ErrorCategory.PatientPracticeMessages;
        
        public IActionResult Visit(PostSendMessageResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(PostSendMessageResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(PostSendMessageResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(PostSendMessageResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(PostSendMessageResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}