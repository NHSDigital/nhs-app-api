using System.Threading.Tasks;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareService : ISecondaryCareService
    {
        private readonly SecondaryCareSummaryService _summaryService;
        private readonly SecondaryCareWaitTimesService _waitTimesService;

        public SecondaryCareService(SecondaryCareSummaryService summaryService, SecondaryCareWaitTimesService waitTimesService)
        {
            _summaryService = summaryService;
            _waitTimesService = waitTimesService;
        }

        public async Task<SecondaryCareSummaryResult> GetSummary(P9UserSession userSession)
            => await _summaryService.GetSummary(userSession);

        public async Task<SecondaryCareWaitTimesResult> GetWaitTimes(P9UserSession userSession)
            => await _waitTimesService.GetWaitTimes(userSession);
    }
}