using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRules.Common;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems
{

    public class OdsCodeLookup : IOdsCodeLookup
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<OdsCodeLookup> _logger;
        private readonly IServiceJourneyRulesClient _serviceJourneyRulesClient;

        public OdsCodeLookup(
            IGpSystemFactory gpSystemFactory,
            ILogger<OdsCodeLookup> logger,
            IServiceJourneyRulesClient serviceJourneyRulesClient)
        {
            _gpSystemFactory = gpSystemFactory ?? throw new ArgumentNullException(nameof(gpSystemFactory));
            _logger = logger;
            _serviceJourneyRulesClient = serviceJourneyRulesClient;
        }  
        
        public async Task<Option<Supplier>> LookupSupplier(string odsCode)
        {
            _logger.LogInformation("Looking up ODS Code {odsCode}", odsCode);

            if (string.IsNullOrWhiteSpace(odsCode))
            {
                return Option.None<Supplier>();
            }

            var supplier = await GetSupplierFromServiceJourneyRules(odsCode);

            if (supplier==Supplier.Unknown)
            {
                _logger.LogError($"Ods code {odsCode} could not be matched to a supported GP System {supplier}");
                return Option.None<Supplier>();
            }

            return Option.Some(supplier);
        }

        private async Task<Supplier> GetSupplierFromServiceJourneyRules(string odsCode)
        {
            try
            {
                _logger.LogEnter();

                var rules = await _serviceJourneyRulesClient.GetServiceJourneyRules(odsCode);
                if(rules.HasSuccessResponse && rules.Body?.Journeys != null)
                {
                    return rules.Body.Journeys.Supplier;
                }

                return Supplier.Unknown;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}