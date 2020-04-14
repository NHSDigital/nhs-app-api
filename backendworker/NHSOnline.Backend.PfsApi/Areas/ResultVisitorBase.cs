using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas
{
    public abstract class ResultVisitorBase
    {
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        protected P9UserSession UserSession { get; }

        protected abstract ErrorCategory ErrorCategory { get; }

        protected ResultVisitorBase(
            IErrorReferenceGenerator errorReferenceGenerator,
            P9UserSession userSession)
        {
            _errorReferenceGenerator = errorReferenceGenerator;
            UserSession = userSession;
        }
        
        protected IActionResult BuildErrorResult(int statusCode)
        {
            var serviceDeskReference =
                _errorReferenceGenerator.GenerateAndLogErrorReference(
                    ErrorCategory, statusCode,
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