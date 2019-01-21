using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Linkage
{
    public class EmisLinkageValidationService : LinkageValidationService
    {
        private readonly ILogger<EmisLinkageValidationService> _logger;

        public EmisLinkageValidationService(ILogger<EmisLinkageValidationService> logger) : base(logger)
        {
            _logger = logger;
        }

        protected override bool IsSupplierGetValid(GetLinkageRequest request)
        {
            return new ValidateAndLog(_logger)
                   .IsNotNullOrWhitespace(request.IdentityToken, nameof(request.IdentityToken))
                   .IsValid();
        }

        protected override bool IsSupplierPostValid(CreateLinkageRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.IdentityToken, nameof(request.IdentityToken))
                .IsNotNullOrWhitespace(request.EmailAddress, nameof(request.EmailAddress))
                .IsValid();
        }
    }
}