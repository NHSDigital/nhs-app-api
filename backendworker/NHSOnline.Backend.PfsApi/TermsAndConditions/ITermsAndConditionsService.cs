using System;
using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    public interface ITermsAndConditionsService
    {
        Task<TermsAndConditionsRecordConsentResult> RecordConsent(string nhsLoginId, ConsentRequest request, DateTimeOffset consentTime);
        Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsLoginId);
        Task<ToggleAnalyticsCookieAcceptanceResult> ToggleAnalyticsCookieAcceptance(string nhsLoginId, AnalyticsCookieAcceptance analyticsCookieConsent, DateTimeOffset consentTime);
    }
}