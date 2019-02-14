using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Linkage
{
    public class VisionLinkageRequestValidationService : ILinkageRequestValidationService
    {
        private readonly ILogger<VisionLinkageRequestValidationService> _logger;

        public VisionLinkageRequestValidationService(ILogger<VisionLinkageRequestValidationService> logger)
        {
            _logger = logger;
        }

        public bool Validate(GetLinkageRequest request)
        {
            var validator = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.NhsNumber, Constants.HttpHeaders.NhsNumber)
                .IsNotNullOrWhitespace(request.OdsCode, Constants.HttpHeaders.OdsCode)
                .IsNotNullOrWhitespace(request.Surname, Constants.HttpHeaders.Surname)
                .IsNotNull(request.DateOfBirth, Constants.HttpHeaders.DateOfBirth);

            return validator.IsValid();
        }

        public bool Validate(CreateLinkageRequest request)
        {
            var validator = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.NhsNumber, nameof(request.NhsNumber))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.Surname, nameof(request.Surname))
                .IsNotNull(request.DateOfBirth, nameof(request.DateOfBirth));

            return validator.IsValid();
        }
    }
}