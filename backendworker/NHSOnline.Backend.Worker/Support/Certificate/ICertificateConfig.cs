namespace NHSOnline.Backend.Worker.Support.Certificate
{
    public interface ICertificateConfig
    {
        string CertificatePath { get; }
        string CertificatePassphrase { get; }
    }
}