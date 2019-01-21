using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Linkage.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Linkage
{
    public interface ILinkageValidationService
    {
        bool IsGetValid(GetLinkageRequest request);

        bool IsPostValid(CreateLinkageRequest request);
    }

    public abstract class LinkageValidationService : ILinkageValidationService
    {
        private readonly ILogger _logger;

        protected LinkageValidationService(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsGetValid(GetLinkageRequest request)
        {
            var baseRequestValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.NhsNumber, nameof(request.NhsNumber))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsValidOdsCode(request.OdsCode, nameof(request.OdsCode))
                .IsValid();

            var supplierSpecificRequestValid = IsSupplierGetValid(request);

            return baseRequestValid && supplierSpecificRequestValid;
        }

        protected abstract bool IsSupplierGetValid(GetLinkageRequest request);
        
        public bool IsPostValid(CreateLinkageRequest request)
        {
            var baseRequestValid = new ValidateAndLog(_logger)
                .IsValidOdsCode(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.NhsNumber, nameof(request.NhsNumber))
                .IsNotNull(request.DateOfBirth, nameof(request.DateOfBirth))
                .IsValid();

            var supplierSpecificRequestValid = IsSupplierPostValid(request);

            return baseRequestValid && supplierSpecificRequestValid;
        }

        protected abstract bool IsSupplierPostValid(CreateLinkageRequest request);
    }
}