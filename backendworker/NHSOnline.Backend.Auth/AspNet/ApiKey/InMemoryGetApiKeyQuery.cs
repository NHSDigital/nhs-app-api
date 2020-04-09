using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Auth.AspNet.ApiKey
{
    public class InMemoryGetApiKeyQuery : IGetApiKeyQuery
    {
        private readonly IDictionary<string, SecureApiKey> _apiKeys;

        public InMemoryGetApiKeyQuery(IApiKeyConfig config)
        {
            _apiKeys = config.ValidSecureApiKeys.ToDictionary(x => x.Key, x => x);
        }

        public Task<SecureApiKey> Execute(string providedApiKey)
        {
            _apiKeys.TryGetValue(providedApiKey, out var key);
            return Task.FromResult(key);
        }
    }
}