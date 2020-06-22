using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    public class TermsAndConditionsFetchConsentResultResponseVisitor
    {
        public ConsentResponse Visit(TermsAndConditionsFetchConsentResult.Success result)
        {
            return result.Response;
        }
    }
}