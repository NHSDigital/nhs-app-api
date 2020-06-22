using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class TermsAndConditionsToConsentResponseMapper :
        IMapper<TermsAndConditionsRecord,ConsentResponse>
    {
        private readonly ILogger<TermsAndConditionsService> _logger;
        private readonly ITermsAndConditionsConfiguration _configuration;

        public TermsAndConditionsToConsentResponseMapper(ILogger<TermsAndConditionsService> logger,
            ITermsAndConditionsConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public ConsentResponse Map(TermsAndConditionsRecord source)
        {
            _logger.LogInformation("Patient consent found for terms and conditions");

            var dateOfConsent = DateTimeOffset.Parse(source.DateOfConsent, CultureInfo.InvariantCulture);

            //Updated consent required if date of last consent is prior to date of updated terms
            var updatedConsentRequired = _configuration.EffectiveDate <= DateTimeOffset.Now
                                         && _configuration.EffectiveDate > dateOfConsent;

            var response = new ConsentResponse
            {
                ConsentGiven = source.ConsentGiven,
                UpdatedConsentRequired = updatedConsentRequired,
                AnalyticsCookieAccepted = source.AnalyticsCookieAccepted,
                DateOfConsent = dateOfConsent
            };

            _logger.LogDebug($"{nameof(source.ConsentGiven)}: {source.ConsentGiven}, " +
                             $"{nameof(response.UpdatedConsentRequired)}: {updatedConsentRequired}, " +
                             $"{nameof(source.AnalyticsCookieAccepted)}: {source.AnalyticsCookieAccepted}");
            return response;
        }
    }
}