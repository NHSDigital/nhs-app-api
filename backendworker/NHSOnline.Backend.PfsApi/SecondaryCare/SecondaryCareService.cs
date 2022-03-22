namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareService : ISecondaryCareService
    {
        private readonly SecondaryCareSummaryService _summaryService;

        public SecondaryCareService(SecondaryCareSummaryService summaryService)
        {
            _summaryService = summaryService;
        }

        public SecondaryCareSummaryResult GetSummary() => _summaryService.GetSummary();
    }
}