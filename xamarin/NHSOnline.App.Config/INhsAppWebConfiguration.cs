using System;

namespace NHSOnline.App.Config
{
    public interface INhsAppWebConfiguration
    {
        string Scheme { get; }
        string Host { get; }
        int Port { get; }
        Uri BaseAddress => new UriBuilder(Scheme, Host, Port).Uri;
    }
}