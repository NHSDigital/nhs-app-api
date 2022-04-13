using System;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public interface INhsApimConfig
    {
        Uri BaseUrl { get; }

        string CertPath { get; }

        string CertPassphrase { get; }

        string Key { get; }

        string Kid { get; }
    }
}