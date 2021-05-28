using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdSigningKeysProvider : ICitizenIdSigningKeysProvider
    {
        private readonly ICitizenIdClient _citizenIdClient;
        private readonly ILogger<CitizenIdSigningKeysProvider> _logger;
        private readonly ConcurrentDictionary<string, JsonWebKeySet> _keyCache;

        public CitizenIdSigningKeysProvider(ICitizenIdClient citizenIdClient,
            ILogger<CitizenIdSigningKeysProvider> logger)
        {
            _citizenIdClient = citizenIdClient;
            _logger = logger;
            _keyCache = new ConcurrentDictionary<string, JsonWebKeySet>();
        }

        public async Task<Option<JsonWebKeySet>> GetSigningKeys(string keyId)
        {
            try
            {
                _logger.LogEnter();

                if (_keyCache.TryGetValue(keyId, out var existingCachedKeySet))
                {
                    return Option.Some(existingCachedKeySet);
                }

                if (!await UpdateKeyCache())
                {
                    return Option.None<JsonWebKeySet>();
                }

                if (_keyCache.TryGetValue(keyId, out var newCachedKeySet))
                {
                    return Option.Some(newCachedKeySet);
                }

                _logger.LogError($"Requested key id '{keyId}' was not found in CID signing keys " +
                                 $"response, keys returned: {string.Join(",", _keyCache.Keys)}");

                return Option.None<JsonWebKeySet>();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,"HttpRequestException getting signing keys from citizen Id");
                return Option.None<JsonWebKeySet>();
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Unexpected exception getting signing keys from citizen Id");
                return Option.None<JsonWebKeySet>();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<bool> UpdateKeyCache()
        {
            var response = await _citizenIdClient.GetSigningKeys();

            if (!response.HasSuccessStatusCode)
            {
                _logger.LogError(
                    $"Retrieving citizen id signingKeys failed with status code {response.StatusCode}");

                return false;
            }

            if (response.Body?.Keys is null)
            {
                _logger.LogError("Unable to parse signing keys from citizen Id response");

                return false;
            }

            foreach (var key in response.Body.Keys)
            {
                var cidKeyId = key?.Kid;

                if (cidKeyId != null)
                {
                    _keyCache[cidKeyId] = response.Body;
                }
            }

            return true;
        }
    }
}
