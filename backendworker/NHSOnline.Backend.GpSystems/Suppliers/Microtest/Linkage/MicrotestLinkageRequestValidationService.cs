using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Linkage
{
    public class MicrotestLinkageValidationService : LinkageValidationService
    {
        private readonly ILogger<MicrotestLinkageValidationService> _logger;

        public MicrotestLinkageValidationService(ILogger<MicrotestLinkageValidationService> logger) : base(logger)
        {
            _logger = logger;
        }

        protected override bool IsSupplierGetValid(GetLinkageRequest request)
        {
            var validator = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.NhsNumber, Constants.HttpHeaders.NhsNumber)
                .IsNotNullOrWhitespace(request.OdsCode, Constants.HttpHeaders.OdsCode)
                .IsNotNullOrWhitespace(request.Surname, Constants.HttpHeaders.Surname)
                .IsNotNull(request.DateOfBirth, Constants.HttpHeaders.DateOfBirth);

            return validator.IsValid();
        }

        protected override bool IsSupplierPostValid(CreateLinkageRequest request)
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
