using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace NHSOnline.Backend.Support.Certificate
{
    public interface ICertificateService
    {
        X509Certificate2 GetCertificate(string certificatePath, string certificatePassphrase);

        bool ServerCertificateValidationHandler(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors);

        void LogCertInfo(string intro, X509Certificate certificate, bool logAsDebug = false);
    }
}