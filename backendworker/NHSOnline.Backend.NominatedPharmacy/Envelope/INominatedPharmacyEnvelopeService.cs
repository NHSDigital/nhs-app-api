namespace NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions
{
    public interface INominatedPharmacyEnvelopeService
    {
        string BuildEnvelope<T>(T request, IServiceDefinition serviceDefinition);
    }
}
