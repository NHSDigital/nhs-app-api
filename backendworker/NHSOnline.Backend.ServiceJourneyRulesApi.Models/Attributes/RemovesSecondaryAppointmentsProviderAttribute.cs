namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models.Attributes
{
    public sealed class RemovesSecondaryAppointmentsProviderAttribute : RemovesSilverIntegrationAttribute
    {
        public RemovesSecondaryAppointmentsProviderAttribute(SecondaryAppointmentsProvider provider) : base(provider.ToString())
        {
        }
    }
}
