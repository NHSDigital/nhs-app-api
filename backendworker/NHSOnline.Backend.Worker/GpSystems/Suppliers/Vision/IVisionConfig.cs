using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionConfig
    {
        string ApplicationProviderId { get; }
        Uri ApiUrl { get; }
        string CertificatePath { get; }
        string CertificatePassphrase { get; }
        string RequestUsername { get; }
    }
}
