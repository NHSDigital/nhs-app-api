using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.SpineSearch;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi
{
    public class SpinePdsConfigurationBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SpinePdsConfigurationBackgroundService> _logger;
        private readonly ISpineSearchService _spineSearchService;
        private readonly INominatedPharmacyConfigurationSettings _nominatedPharmacyConfigurationSettings;

        private readonly int maxSpineRetryWaitTimeSeconds = 28800; // 8 hours
        private int _spineRetryWaitTimeSeconds;

        public SpinePdsConfigurationBackgroundService(
            IConfiguration configuration,
            ILogger<SpinePdsConfigurationBackgroundService> logger,
            ISpineSearchService spineSearchService,
            INominatedPharmacyConfigurationSettings nominatedPharmacyConfigurationSettings)
        {
            _configuration = configuration;
            _logger = logger;
            _spineSearchService = spineSearchService;
            _nominatedPharmacyConfigurationSettings = nominatedPharmacyConfigurationSettings;

            var parsedSpineRetryWaitTimeSeconds = _configuration.GetIntOrDefault("SPINE_LDAP_CONNECTION_RETRY_WAIT_TIME_SECONDS", _logger);
            _spineRetryWaitTimeSeconds = parsedSpineRetryWaitTimeSeconds > 0 ? parsedSpineRetryWaitTimeSeconds : 30;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(SpinePdsConfigurationBackgroundService)} is starting.");

            stoppingToken.Register(() =>
                _logger.LogInformation($"{nameof(SpinePdsConfigurationBackgroundService)} background task is stopping via CancellationToken."));

            var isSpineLdapLookupEnabled = bool.TrueString.Equals(_configuration.GetOrWarn("SPINE_LDAP_LOOKUP_ENABLED", _logger), StringComparison.OrdinalIgnoreCase);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"{nameof(SpinePdsConfigurationBackgroundService)} background task doing work.");

                NhsAppSpinePdsUpdateProperties nhsAppSpinePdsUpdateProperties;

                if (isSpineLdapLookupEnabled)
                {
                    nhsAppSpinePdsUpdateProperties = _spineSearchService.RetrieveSpinePropertiesForPdsUpdate();

                    if (!nhsAppSpinePdsUpdateProperties.Validate())
                    {
                        _logger.LogInformation($"Error retrieving Spine properties for PDS. Waiting {_spineRetryWaitTimeSeconds} seconds then trying again.");

                        await Task.Delay(_spineRetryWaitTimeSeconds * 1000, stoppingToken);

                        UpdateSpineRetryWaitTime();

                        continue;
                    }
                }
                else
                {
                    nhsAppSpinePdsUpdateProperties = new NhsAppSpinePdsUpdateProperties
                    {
                        FromAsid = _configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                        ToAsid = _configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                        CpaId = _configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                        ToPartyId = _configuration.GetOrWarn("TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", _logger),
                    };
                }

                UpdateNominatedPharmacyConfig(nhsAppSpinePdsUpdateProperties);

                _logger.LogInformation($"{nameof(SpinePdsConfigurationBackgroundService)} background task has updated nominated pharmacy config.");
                break;
            }

            _logger.LogInformation($"{nameof(SpinePdsConfigurationBackgroundService)} background task is stopping.");
        }

        private void UpdateNominatedPharmacyConfig(NhsAppSpinePdsUpdateProperties nhsAppSpinePdsUpdateProperties)
        {
            var isNominatedPharmacyEnabled = bool.TrueString.Equals(_configuration.GetOrWarn("NOMINATED_PHARMACY_ENABLED", _logger), StringComparison.OrdinalIgnoreCase);
            if (isNominatedPharmacyEnabled)
            {
                var nominatedPharmacyUriString = _configuration.GetOrWarn("SPINE_PDS_URL", _logger);
                var nominatedPharmacyUriPathString = _configuration.GetOrWarn("SPINE_PDS_URL_PATH", _logger);
                var pdsQueryFromAddress = _configuration.GetOrWarn("PDS_QUERY_FROM_ADDRESS", _logger);
                var pdsQueryTo = _configuration.GetOrWarn("PDS_QUERY_TO", _logger);
                var artificialDelayAfterNominatedPharmacyUpdateInMilliseconds = _configuration.GetIntOrDefault("DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS", _logger);

                var pdsTraceConfigurationSettings = new PdsTraceConfigurationSettings
                {
                    FromAddress = pdsQueryFromAddress,
                    ToAddress = pdsQueryTo,
                    FromAsid = nhsAppSpinePdsUpdateProperties.FromAsid,
                    ToAsid = nhsAppSpinePdsUpdateProperties.ToAsid,
                };

                var pdsUpdateConfigurationSettings = new PdsUpdateConfigurationSettings
                {
                    FromAsid = nhsAppSpinePdsUpdateProperties.FromAsid,
                    ToAsid = nhsAppSpinePdsUpdateProperties.ToAsid,
                    CpaId = nhsAppSpinePdsUpdateProperties.CpaId,
                    FromPartyId = _configuration.GetOrWarn("NHS_APP_PARTY_ID_FOR_SPINE", _logger),
                    ToPartyId = nhsAppSpinePdsUpdateProperties.ToPartyId,
                };

                _nominatedPharmacyConfigurationSettings.Update(
                    isNominatedPharmacyEnabled,
                    new Uri(nominatedPharmacyUriString, UriKind.Absolute),
                    nominatedPharmacyUriPathString,
                    artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
                    pdsTraceConfigurationSettings,
                    pdsUpdateConfigurationSettings
                    );

                var nominatedPharmacyConfigIsValid = _nominatedPharmacyConfigurationSettings.Validate();
                if (!nominatedPharmacyConfigIsValid)
                {
                    _logger.LogWarning($"Not all nominated pharmacy config is populated, disabling nominated pharmacy feature (initial value was {isNominatedPharmacyEnabled})");
                    _nominatedPharmacyConfigurationSettings.IsNominatedPharmacyEnabled = false;
                }
            }
        }

        private void UpdateSpineRetryWaitTime()
        {
            if (_spineRetryWaitTimeSeconds >= maxSpineRetryWaitTimeSeconds)
            {
                return;
            }

            // double the time previously waited for the next attempt
            _spineRetryWaitTimeSeconds *= 2;

            if (_spineRetryWaitTimeSeconds >= maxSpineRetryWaitTimeSeconds)
            {
                _spineRetryWaitTimeSeconds = maxSpineRetryWaitTimeSeconds;
            }
        }
    }
}
