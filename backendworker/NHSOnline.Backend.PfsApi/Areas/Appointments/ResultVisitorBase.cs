using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public abstract class ResultVisitorBase
    {
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        protected UserSession UserSession { get; }
        
        protected ResultVisitorBase(
            IErrorReferenceGenerator errorReferenceGenerator,
            UserSession userSession)
        {
            _errorReferenceGenerator = errorReferenceGenerator;
            UserSession = userSession;
        }
        
        protected IActionResult BuildErrorResult(int statusCode)
        {
            var serviceDeskReference =
                _errorReferenceGenerator.GenerateAndLogErrorReference(
                    ErrorCategory.Appointments, statusCode,
                    UserSession.GpUserSession.Supplier);

            return new ObjectResult(new PfsErrorResponse
            {
                ServiceDeskReference = serviceDeskReference
            })
            {
                StatusCode = statusCode
            };
        }
    }
}