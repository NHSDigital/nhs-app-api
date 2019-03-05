using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Support.Settings;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Temporal;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.CitizenId
{
    public class CitizenIdSessionService : ICitizenIdSessionService
    {
        private readonly ICitizenIdService _citizenIdService;
        private readonly IMinimumAgeValidator _minimumAgeValidator;
        private readonly IOptions<ConfigurationSettings> _settings;
        private readonly ILogger<CitizenIdSessionService> _logger;
        private const string DateFormat = "yyyy-MM-dd";
        
        public CitizenIdSessionService(ICitizenIdService citizenIdService, IMinimumAgeValidator minimumAgeValidator, 
            IOptions<ConfigurationSettings> settings, ILogger<CitizenIdSessionService> logger)
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
                    return new CitizenIdSessionResult()
                    {
                        StatusCode = (int) userProfileResult.StatusCode
                    };
                }

                var cidUserProfile = cidUserProfileOption.ValueOrFailure();

                // Validate the Date of Birth meets expected format and minimum age requirements
                var dateOfBirthParsed = ValidateAndParseDateOfBirth(cidUserProfile.DateOfBirth);
                if (!dateOfBirthParsed.HasValue)
                {
                    return new CitizenIdSessionResult()
                    {
                        StatusCode = Constants.CustomHttpStatusCodes.Status465FailedAgeRequirement
                    };
                }

                // Validate the NHS number has been returned from CID.
                var nhsNumberFormatted = cidUserProfile.NhsNumber.FormatToNhsNumber();
                if (string.IsNullOrEmpty(nhsNumberFormatted))
                {
                    _logger.LogError($"No NHS number was found");
                    return new CitizenIdSessionResult()
                    {
                        StatusCode = Constants.CustomHttpStatusCodes.Status464OdsCodeNotSupportedOrNoNhsNumber
                    };
                }

                return new CitizenIdSessionResult()
                {
                    DateOfBirth = dateOfBirthParsed.Value,
                    Im1ConnectionToken = cidUserProfile.Im1ConnectionToken,
                    OdsCode = cidUserProfile.OdsCode,
                    StatusCode = (int) userProfileResult.StatusCode,
                    NhsNumber = nhsNumberFormatted,
                    Session = new CitizenIdUserSession()
                    {
                        AccessToken = cidUserProfile.AccessToken
                    }
                };
            }
            finally
            {
                _logger.LogExit();
            }
        }
        
        private DateTime? ValidateAndParseDateOfBirth(string dateOfBirth)
        {
            if (!DateTime.TryParseExact(dateOfBirth, DateFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dateOfBirthParsed))
            {
                _logger.LogError("Missing or invalid date of birth");
                return null;
            }

            if (!_minimumAgeValidator.IsValid(dateOfBirthParsed, _settings.Value.MinimumAppAge))
            {
                _logger.LogError("Failed to meet the minimum age requirement.");
                return null;
            }

            return dateOfBirthParsed;
        }
    }
}