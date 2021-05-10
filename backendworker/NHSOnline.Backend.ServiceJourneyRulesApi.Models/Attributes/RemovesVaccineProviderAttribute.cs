namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models.Attributes
{
    public sealed class RemovesVaccineProviderAttribute : RemovesSilverIntegrationAttribute
    {
        public RemovesVaccineProviderAttribute(VaccineRecordProvider provider) : base(provider.ToString())
        {
        }
    }
}
