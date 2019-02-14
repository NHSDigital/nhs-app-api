using System.Security.Cryptography.X509Certificates;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope
{
    public interface IEnvelopeService
    {
        string BuildEnvelope<T>(X509Certificate2 certificate, T request, string requestUsername);
    }
}