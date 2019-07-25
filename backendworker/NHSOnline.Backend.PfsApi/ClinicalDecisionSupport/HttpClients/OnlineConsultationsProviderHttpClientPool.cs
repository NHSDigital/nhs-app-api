using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public class OnlineConsultationsProviderHttpClientPool : IOnlineConsultationsProviderHttpClientPool
    {
        private readonly ILogger<IOnlineConsultationsProviderHttpClientPool> _logger;
        private readonly Dictionary<string, IOnlineConsultationsProviderHttpClient> _pool;
        
        public OnlineConsultationsProviderHttpClientPool(
            OnlineConsultationsProvidersSettings onlineConsultationsProvidersSettings,
            ILoggerFactory loggerFactory,
            ILogger<IOnlineConsultationsProviderHttpClientPool> logger)
        {
            _logger = logger;
            _pool = new Dictionary<string, IOnlineConsultationsProviderHttpClient>();

            foreach (var providerSettings in onlineConsultationsProvidersSettings.Providers)
            {
                if (!_pool.ContainsKey(providerSettings.Provider)) {
                    _pool.Add(
                        providerSettings.Provider,
                        new OnlineConsultationsProviderHttpClient(
                            new HttpClient(),
                            providerSettings,
                            loggerFactory.CreateLogger<OnlineConsultationsProviderHttpClient>()));
               }
            }
        }
        
        public IOnlineConsultationsProviderHttpClient GetClientByProviderName(string provider)
        {
            try
            {
                return _pool[provider];
            }
            catch (ArgumentNullException)
            {
                _logger.LogError("Provider key is null");

                return null;
            }
            catch (KeyNotFoundException)
            {
                _logger.LogError($"HttpClient not found for {provider}");

                return null;
            }
        }
    }
}