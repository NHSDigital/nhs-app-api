using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;

namespace NHSOnline.Backend.Worker.TermsAndConditions
{
    public interface ITermsAndConditionsService
    {       
        Task<TermsAndConditionsRecordConsentResult> RecordConsent(string nhsNumber, ConsentRequest request);
        Task<TermsAndConditionsFetchConsentResult> FetchConsent(string nhsNumber);
    }
}