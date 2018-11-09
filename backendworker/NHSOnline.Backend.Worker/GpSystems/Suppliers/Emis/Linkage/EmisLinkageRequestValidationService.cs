using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageRequestValidationService : ILinkageRequestValidationService
    {
        private readonly ILogger<EmisLinkageRequestValidationService> _logger;

        public EmisLinkageRequestValidationService(ILogger<EmisLinkageRequestValidationService> logger)
        {
            _logger = logger;
        }

        public bool Validate(GetLinkageRequest request)
        {
            var validator = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.NhsNumber,Constants.HttpHeaders.NhsNumber)
                .IsNotNullOrWhitespace(request.OdsCode,Constants.HttpHeaders.OdsCode)
                .IsNotNullOrWhitespace(request.IdentityToken,Constants.HttpHeaders.IdentityToken);

            return validator.IsValid();
        }

        public bool Validate(CreateLinkageRequest request)
        {
            var validator = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.NhsNumber,nameof(request.NhsNumber))
                .IsNotNullOrWhitespace(request.OdsCode,nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.IdentityToken,nameof(request.IdentityToken))
                .IsNotNullOrWhitespace(request.EmailAddress,nameof(request.EmailAddress));

            return validator.IsValid();
        }
    }
}