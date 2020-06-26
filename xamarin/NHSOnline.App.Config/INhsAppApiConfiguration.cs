using System;

namespace NHSOnline.App.Config
{
    public interface INhsAppApiConfiguration
    {
        string Scheme { get; }
        string Host { get; }
        int Port { get; }
        Uri BaseAddress => new UriBuilder(Scheme, Host, Port).Uri;
    }
}