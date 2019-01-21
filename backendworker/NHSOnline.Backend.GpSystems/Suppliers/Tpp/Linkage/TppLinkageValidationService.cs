using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Linkage
{
    public class TppLinkageValidationService : LinkageValidationService
    {
        private readonly ILogger<TppLinkageValidationService> _logger;

        public TppLinkageValidationService(ILogger<TppLinkageValidationService> logger) : base(logger)
        {
            _logger = logger;
        }

        protected  override bool IsSupplierGetValid(GetLinkageRequest request)
        {
            var validator = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.Surname, Constants.HttpHeaders.Surname)
                .IsNotNull(request.DateOfBirth, Constants.HttpHeaders.DateOfBirth);

            return validator.IsValid();
        }

        protected override bool IsSupplierPostValid(CreateLinkageRequest request)
        {
            return new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.Surname, nameof(request.Surname))
                .IsValid();
        }
    }
}