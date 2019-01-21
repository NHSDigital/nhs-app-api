using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public class VisionLinkageValidationService : LinkageValidationService
    {
        private readonly ILogger<VisionLinkageValidationService> _logger;

        public VisionLinkageValidationService(ILogger<VisionLinkageValidationService> logger) : base(logger)
        {
            _logger = logger;
        }

        protected override bool IsSupplierGetValid(GetLinkageRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.Surname, Constants.HttpHeaders.Surname)
                .IsNotNull(request.DateOfBirth, Constants.HttpHeaders.DateOfBirth)
                .IsValid();
        }

        protected override bool IsSupplierPostValid(CreateLinkageRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.Surname, nameof(request.Surname))
                .IsNotNull(request.DateOfBirth, nameof(request.DateOfBirth))
                .IsValid();
        }
    }
}