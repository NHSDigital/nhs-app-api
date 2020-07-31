using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas
{
    public abstract class ResultVisitorBase
    {
        private readonly IErrorReferenceGenerator _errorReferenceGenerator;

        protected Supplier Supplier { get; }

        protected abstract ErrorCategory ErrorCategory { get; }

        protected ResultVisitorBase(
            IErrorReferenceGenerator errorReferenceGenerator,
            Supplier supplier)
        {
            _errorReferenceGenerator = errorReferenceGenerator;
            Supplier = supplier;
        }

        protected ResultVisitorBase(
            IErrorReferenceGenerator errorReferenceGenerator,
            P9UserSession userSession)
        {
            _errorReferenceGenerator = errorReferenceGenerator;
            Supplier = userSession.GpUserSession.Supplier;
        }

        protected IActionResult BuildErrorResult(int statusCode)
        {
            var serviceDeskReference =
                _errorReferenceGenerator.GenerateAndLogErrorReference(
                    ErrorCategory, statusCode,
                    Supplier);

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