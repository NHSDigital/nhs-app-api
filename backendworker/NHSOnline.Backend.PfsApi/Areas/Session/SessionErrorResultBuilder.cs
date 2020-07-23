using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    public class SessionErrorResultBuilder : ISessionErrorResultBuilder
    {
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        public SessionErrorResultBuilder(IErrorReferenceGenerator errorReferenceGenerator)
        {
            _errorReferenceGenerator = errorReferenceGenerator;
        }

        public IActionResult BuildResult(ErrorTypes errorTypes)
        {
            var serviceDeskReference = _errorReferenceGenerator.GenerateAndLogErrorReference(errorTypes);

            return new ObjectResult(new PfsErrorResponse { ServiceDeskReference = serviceDeskReference })
            {
                StatusCode = errorTypes.StatusCode
            };
        }
    }
}