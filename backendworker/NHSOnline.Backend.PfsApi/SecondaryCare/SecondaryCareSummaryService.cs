namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public class SecondaryCareSummaryService
    {
        private readonly ISecondaryCareClient _secondaryCareClient;

        public SecondaryCareSummaryService(ISecondaryCareClient secondaryCareClient)
        {
            _secondaryCareClient = secondaryCareClient;
        }

        public SecondaryCareSummaryResult GetSummary()
        {
            var summaryResponse = _secondaryCareClient.GetSummary();

            if (summaryResponse.HasSuccessResponse)
            {
                return new SecondaryCareSummaryResult.Success(summaryResponse.Body);
            }

            return new SecondaryCareSummaryResult.BadGateway();
        }
    }
}