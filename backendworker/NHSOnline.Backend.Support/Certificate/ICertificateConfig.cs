namespace NHSOnline.Backend.Support.Certificate
{
    public interface ICertificateConfig
    {
        string CertificatePath { get; }
        string CertificatePassphrase { get; }
    }
}