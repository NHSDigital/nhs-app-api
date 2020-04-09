using System.Collections.Generic;

namespace NHSOnline.Backend.Auth.AspNet.ApiKey
{
    public class ApiKeyConfig : IApiKeyConfig
    {
        public ApiKeyConfig(IEnumerable<SecureApiKey> secureApiKeys)
        {
            ValidSecureApiKeys = secureApiKeys;
        }

        public IEnumerable<SecureApiKey> ValidSecureApiKeys { get; set; }
    }
}