using System.Collections.Generic;

namespace NHSOnline.Backend.Auth.AspNet.ApiKey
{
    public interface IApiKeyConfig
    {
        IEnumerable<SecureApiKey> ValidSecureApiKeys { get; set; }
    }
}