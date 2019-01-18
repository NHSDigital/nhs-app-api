using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Linkage.Models;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Linkage
{
    public class MicrotestLinkageRequestValidationService : ILinkageRequestValidationService
    {
        private readonly ILogger<MicrotestLinkageRequestValidationService> _logger;

        public MicrotestLinkageRequestValidationService(ILogger<MicrotestLinkageRequestValidationService> logger)
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
