using System;
using System.Globalization;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    internal class ConsentRequestToTermsAndConditionsMapper : IConsentRequestToTermsAndConditionsMapper
    {
        public TermsAndConditionsRecord Map(ConsentRequest request, DateTimeOffset consentTime, string nhsLoginId)
        {
            var consentTimeString = consentTime.ToString("s", CultureInfo.InvariantCulture);

            var termsAndConditions = new TermsAndConditionsRecord
            {
                NhsLoginId = nhsLoginId,
                ConsentGiven = request.ConsentGiven,
                AnalyticsCookieAccepted = request.AnalyticsCookieAccepted,
                DateOfConsent = consentTimeString,
                DateOfAnalyticsCookieToggle = consentTimeString
            };
            return termsAndConditions;
        }
    }
}