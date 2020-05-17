using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;

namespace NHSOnline.Backend.NominatedPharmacy.Envelope
{
    public interface INominatedPharmacyEnvelopeService
    {
        string BuildEnvelope<T>(T request, IServiceDefinition serviceDefinition);
    }
}
