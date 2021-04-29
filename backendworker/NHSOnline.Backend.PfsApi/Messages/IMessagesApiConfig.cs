using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.Messages
{
    public interface IMessagesApiConfig
    {
        string ApiKey { get; set; }
        Uri BaseUrl { get; set; }

        [SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "Url contains a {0} format token")]
        string ResourceUrl { get; set; }
    }
}
