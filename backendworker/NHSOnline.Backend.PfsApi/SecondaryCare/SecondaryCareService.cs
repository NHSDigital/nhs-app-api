using System.Threading.Tasks;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareService : ISecondaryCareService
    {
        private readonly SecondaryCareSummaryService _summaryService;

        public SecondaryCareService(SecondaryCareSummaryService summaryService)
        {
            _summaryService = summaryService;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
            => await _summaryService.GetSummary(userSession);
    }
}