using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Auth.CitizenId;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public class CitizenIdSessionService : ICitizenIdSessionService
    {
        private readonly ICitizenIdService _citizenIdService;
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private readonly ConfigurationSettings _settings;
        private readonly ILogger<CitizenIdSessionService> _logger;
        private const string DateFormat = "yyyy-MM-dd";

        public CitizenIdSessionService(ICitizenIdService citizenIdService, IMinimumAgeValidator minimumAgeValidator,
            ConfigurationSettings settings, ILogger<CitizenIdSessionService> logger)
        {
            _citizenIdService = citizenIdService;
            _minimumAgeValidator = minimumAgeValidator;
            _settings = settings;
            _logger = logger;
        }

        [SuppressMessage("Microsoft.Design", "CA1054", Justification = "Uris are not serializable")]
        public async Task<CitizenIdSessionResult> Create(string authCode, string codeVerifier, string redirectUrl)
        {
            _logger.LogEnter();

            try
            {
                var userProfileResult = await _citizenIdService.GetUserProfile(authCode, codeVerifier, redirectUrl);
                var cidUserProfileOption = userProfileResult.UserProfile;

                if (!cidUserProfileOption.HasValue)
                {
                    _logger.LogError("No CID profile was found for received authcode and code verifier");
                    return new CitizenIdSessionResult
                    {
                        StatusCode = (int) userProfileResult.StatusCode
                    };
                }

                var cidUserProfile = cidUserProfileOption.ValueOrFailure();

                // Validate the Date of Birth meets expected format and minimum age requirements
                var dateOfBirthParsed = ValidateAndParseDateOfBirth(cidUserProfile.DateOfBirth);
                if (!dateOfBirthParsed.HasValue)
                {
                    return new CitizenIdSessionResult
                    {
                        StatusCode = Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement
                    };
                }

                // Validate the NHS number has been returned from CID.
                var nhsNumberFormatted = cidUserProfile.NhsNumber.FormatToNhsNumber();
                if (string.IsNullOrEmpty(nhsNumberFormatted))
                {
                    _logger.LogError("No NHS number was found");
                    return new CitizenIdSessionResult
                    {
                        StatusCode = Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber
                    };
                }

                var proofLevel = TryParseIdentityProofingLevel(cidUserProfile.IdentityProofingLevel);
                if (!proofLevel.HasValue)
                {
                    return new CitizenIdSessionResult { StatusCode = (int)HttpStatusCode.InternalServerError };
                }

                return new CitizenIdSessionResult
                {
                    DateOfBirth = dateOfBirthParsed.Value,
                    Im1ConnectionToken = cidUserProfile.Im1ConnectionToken,
                    StatusCode = (int) userProfileResult.StatusCode,
                    NhsNumber = nhsNumberFormatted,
                    Session = new CitizenIdUserSession
                    {
                        AccessToken = cidUserProfile.AccessToken,
                        GivenName = cidUserProfile.GivenName,
                        FamilyName = cidUserProfile.FamilyName,
                        OdsCode = cidUserProfile.OdsCode,
                        DateOfBirth = dateOfBirthParsed.Value,
                        IdTokenJti = userProfileResult.IdTokenJti,
                        ProofLevel = proofLevel.Value
                    }
                };
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private ProofLevel? TryParseIdentityProofingLevel(string identityProofingLevel)
        {
            // NHSO-9061: Remove once supported by Login
            if (string.IsNullOrWhiteSpace(identityProofingLevel))
            {
                return ProofLevel.P9;
            }

            if (Enum.TryParse(typeof(ProofLevel), identityProofingLevel, true, out var proofLevel))
            {
                return (ProofLevel)proofLevel;
            }

            _logger.LogError($"Unsupported identity proofing level returned by Login: {identityProofingLevel}");
            return null;
        }

        private DateTime? ValidateAndParseDateOfBirth(string dateOfBirth)
        {
            if (!DateTime.TryParseExact(dateOfBirth, DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dateOfBirthParsed))
            {
                _logger.LogError("Missing or invalid date of birth");
                return null;
            }

            if (!_minimumAgeValidator.IsValid(dateOfBirthParsed, _settings.MinimumAppAge))
            {
                _logger.LogError("Failed to meet the minimum age requirement.");
                return null;
            }

            return dateOfBirthParsed;
        }
    }
}