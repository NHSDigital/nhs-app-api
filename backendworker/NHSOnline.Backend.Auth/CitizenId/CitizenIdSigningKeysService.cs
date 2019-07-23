using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class CitizenIdSigningKeysService : ICitizenIdSigningKeysService
    {
        private readonly ICitizenIdClient _citizenIdClient;
        private readonly ILogger<CitizenIdSigningKeysService> _logger;

        public CitizenIdSigningKeysService(ICitizenIdClient citizenIdClient,ILogger<CitizenIdSigningKeysService> logger)
        {
            _citizenIdClient = citizenIdClient;
            _logger = logger;
        }

        public async Task<Option<JsonWebKeySet>> GetSigningKeys()
        {
            try
            {
                _logger.LogEnter();

                var response = await _citizenIdClient.GetSigningKeys();
                if (response.HasSuccessStatusCode)
                {
                    return Option.Some(response.Body);
                }
                
                _logger.LogError($"Retrieving citizen id signingKeys failed with status code {response.StatusCode}");
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
    }
}