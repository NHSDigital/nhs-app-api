using System.Collections.Generic;
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

        public bool IsPostValid(PatientIm1ConnectionRequest request, out IEnumerable<string> invalidParameters)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsValidOdsCode(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.AccountId, nameof(request.AccountId))
                .IsNotNullOrWhitespace(request.LinkageKey, nameof(request.LinkageKey))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.Surname, nameof(request.Surname))
                .IsNotNull(request.DateOfBirth, nameof(request.DateOfBirth))
                .IsValid(out invalidParameters);

            return isValid;
        }
        
        public bool IsCreateLinkageRequestValid(Im1RegistrationRequest request, out IEnumerable<string> invalidParameters)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.NhsNumber, nameof(request.NhsNumber))
                .IsNotNullOrWhitespace(request.Surname, nameof(request.Surname))
                .IsNotNull(request.DateOfBirth, nameof(request.DateOfBirth))
                .IsValidOdsCode(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.IdentityToken, nameof(request.IdentityToken))
                .IsNotNullOrWhitespace(request.EmailAddress, nameof(request.EmailAddress))
                .IsValid(out invalidParameters);

            return isValid;
        }

        public bool IsPatientIm1ConnectionRequestValid(Im1RegistrationRequest request, out IEnumerable<string> invalidParameters)
        {
            var isValid = new ValidateAndLog(_logger)
                .IsNotNullOrWhitespace(request.AccountId, nameof(request.AccountId))
                .IsNotNull(request.DateOfBirth, nameof(request.DateOfBirth))
                .IsNotNullOrWhitespace(request.LinkageKey, nameof(request.LinkageKey))
                .IsValidOdsCode(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.OdsCode, nameof(request.OdsCode))
                .IsNotNullOrWhitespace(request.Surname, nameof(request.Surname))
                .IsValid(out invalidParameters);

            return isValid;
        }
    }
}
