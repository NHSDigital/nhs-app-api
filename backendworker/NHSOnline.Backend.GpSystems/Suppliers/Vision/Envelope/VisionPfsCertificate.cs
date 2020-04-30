using System;
using System.Security.Cryptography.X509Certificates;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Configuration;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Envelope
{
    internal sealed class VisionPfsCertificate: IValidatable
    {
        public VisionPfsCertificate(
            ICertificateService certificateService,
            VisionConfigurationSettings settings)
        {
            Certificate = certificateService.GetCertificate(settings.CertificatePath, settings.CertificatePassphrase);
        }

        public X509Certificate2 Certificate { get; }

        void IValidatable.Validate()
        {
            if (Certificate == null)
            {
                throw new InvalidOperationException("Vision certificate is required");
            }
        }
    }
}