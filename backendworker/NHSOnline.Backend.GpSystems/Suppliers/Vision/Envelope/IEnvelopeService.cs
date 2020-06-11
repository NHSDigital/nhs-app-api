namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope
{
    public interface IEnvelopeService
    {
        string BuildEnvelope<T>(T request, string requestUsername);
    }
}