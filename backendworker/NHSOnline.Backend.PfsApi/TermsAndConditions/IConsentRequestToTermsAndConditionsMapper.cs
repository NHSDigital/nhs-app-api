using System;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface IConsentRequestToTermsAndConditionsMapper
    {
        TermsAndConditionsRecord Map(ConsentRequest request, DateTimeOffset consentTime, string nhsLoginId);
    }
}