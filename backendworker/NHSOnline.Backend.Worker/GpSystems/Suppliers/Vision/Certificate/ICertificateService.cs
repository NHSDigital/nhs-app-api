using System.Security.Cryptography.X509Certificates;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate
{
    public interface ICertificateService
    {
        X509Certificate2 GetCertificate(string certificatePath, string certificatePassphrase);
    }
}