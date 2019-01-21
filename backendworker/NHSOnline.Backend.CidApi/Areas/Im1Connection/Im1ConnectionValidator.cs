using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{
    public class Im1ConnectionValidator
    {
        private readonly ILogger _logger;

        public Im1ConnectionValidator(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsGetValid(string connectionToken, string odsCode)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsValidOdsCode(odsCode, nameof(odsCode))
                .IsNotNullOrWhitespace(odsCode, nameof(odsCode))
                .IsNotNullOrWhitespace(connectionToken, nameof(connectionToken))
                .IsValid();

            return isValid;
        }

        public bool IsPostValid(PatientIm1ConnectionRequest request)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsValidOdsCode(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.AccountId, nameof(request.AccountId))
                .IsNotNullOrWhitespace(request.LinkageKey, nameof(request.LinkageKey))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.Surname, nameof(request.Surname))
                .IsNotNull(request.DateOfBirth, nameof(request.DateOfBirth))
                .IsValid();

            return isValid;
        }
    }
}
