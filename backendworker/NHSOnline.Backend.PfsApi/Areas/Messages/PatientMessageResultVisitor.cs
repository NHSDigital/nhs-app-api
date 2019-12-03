using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.GpSystems.Messages;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Messages
{
    public class PatientMessageResultVisitor : ResultVisitorBase, IPatientMessageResultVisitor<IActionResult>
    {
        public PatientMessageResultVisitor(IErrorReferenceGenerator errorReferenceGenerator, UserSession userSession) 
            : base(errorReferenceGenerator, userSession){}
        
        protected override ErrorCategory ErrorCategory => ErrorCategory.PatientPracticeMessages;
        
        public IActionResult Visit(GetPatientMessageResult.Success result)
        {
            return new OkObjectResult(result.Response);
        }

        public IActionResult Visit(GetPatientMessageResult.BadRequest result)
        {
            return BuildErrorResult(StatusCodes.Status400BadRequest);
        }

        public IActionResult Visit(GetPatientMessageResult.Forbidden result)
        {
            return BuildErrorResult(StatusCodes.Status403Forbidden);
        }

        public IActionResult Visit(GetPatientMessageResult.BadGateway result)
        {
            return BuildErrorResult(StatusCodes.Status502BadGateway);
        }

        public IActionResult Visit(GetPatientMessageResult.InternalServerError result)
        {
            return BuildErrorResult(StatusCodes.Status500InternalServerError);
        }
    }
}