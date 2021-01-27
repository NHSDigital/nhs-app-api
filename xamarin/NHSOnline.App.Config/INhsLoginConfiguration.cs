using System;

namespace NHSOnline.App.Config
{
    public interface INhsLoginConfiguration
    {
        string Scheme { get; }
        string BaseHost { get; }
        string AuthHost { get; }
        string UafHost { get; }
        int Port { get; }
        string AuthorizePath { get; }

        Uri AuthBaseAddress => new UriBuilder(Scheme, AuthHost, Port, AuthorizePath).Uri;
        Uri UafBaseAddress => new UriBuilder(Scheme, UafHost, Port).Uri;
    }
}