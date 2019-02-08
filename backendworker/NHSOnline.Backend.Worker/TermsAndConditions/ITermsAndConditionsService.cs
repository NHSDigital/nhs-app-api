using System;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.TermsAndConditions.Models;

namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    public interface ITermsAndConditionsService
    {       
        Task<TermsAndConditionsRecordConsentResult> RecordConsent(string nhsNumber, string odsCode, ConsentRequest request, DateTimeOffset termsAndConditionsAcceptanceDate);
        Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsNumber);
    }
}