namespace NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions
{
    public interface IEnvelopeService
    {
        string BuildEnvelope<T>(T request, IServiceDefinition serviceDefinition);
    }
}
