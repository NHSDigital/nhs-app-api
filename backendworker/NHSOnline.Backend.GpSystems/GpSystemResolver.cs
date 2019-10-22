using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public class GpSystemResolver : IGpSystemResolver
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<OdsCodeLookup> _logger;
        private readonly IOdsCodeLookup _odsCodeLookup;

        public GpSystemResolver(
            IGpSystemFactory gpSystemFactory,
            ILogger<OdsCodeLookup> logger,
            IOdsCodeLookup odsCodeLookup)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _odsCodeLookup = odsCodeLookup;
        }
        public async Task<Option<IGpSystem>> ResolveFromOdsCode(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);

            try
            {
                _logger.LogInformation($"Fetch GP System: '{supplier}'.");
                return supplier.Select(_gpSystemFactory.CreateGpSystem);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Failed to create GP System for supplier: {supplier}.");
                return Option.None<IGpSystem>();
            }
        }
    }
}