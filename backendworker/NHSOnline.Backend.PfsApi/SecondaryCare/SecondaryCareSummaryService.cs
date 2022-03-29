using System.Threading.Tasks;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareSummaryService
    {
        private readonly ISecondaryCareClient _secondaryCareClient;

        public SecondaryCareSummaryService(ISecondaryCareClient secondaryCareClient)
        {
            _secondaryCareClient = secondaryCareClient;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
        {
            var summaryResponse = await _secondaryCareClient.GetSummary(userSession);

            if (summaryResponse.HasSuccessResponse)
            {
                return new SecondaryCareSummaryResult.Success(summaryResponse.Body);
            }

            return new SecondaryCareSummaryResult.BadGateway();
        }
    }
}