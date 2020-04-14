using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessagesResultVisitor : ResultVisitorBase, IPatientMessagesResultVisitor<IActionResult>
    {
        public PatientMessagesResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, P9UserSession userSession) 
            : base(errorReferenceGenerator, userSession){}
        
        protected override ErrorCategory ErrorCategory => ErrorCategory.PatientPracticeMessages;
        
        public IActionResult Visit(GetPatientMessagesResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetPatientMessagesResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetPatientMessagesResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(GetPatientMessagesResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetPatientMessagesResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}