using System;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestConfigurationSettings
    {
        public Uri BaseUrl { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassphrase { get; set; }
        public string Environment {get; set;}

        public MicrotestConfigurationSettings(Uri baseUrl, string certificatePath, string certificatePassphrase, string environment)
        {
            BaseUrl = baseUrl;
            CertificatePath = certificatePath;
            CertificatePassphrase = certificatePassphrase;
            Environment = environment;
        }

        public void Validate()
        {
            if(BaseUrl == null)
            {
                throw new ConfigurationNotFoundException("BaseUrl cannot be null");
            }
        }
    }
}
