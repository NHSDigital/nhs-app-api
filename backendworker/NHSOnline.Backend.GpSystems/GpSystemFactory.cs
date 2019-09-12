using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public class GpSystemFactory : IGpSystemFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOdsCodeLookup _odsCodeLookup;
        private readonly ILogger<GpSystemFactory> _logger;

        public GpSystemFactory(IServiceProvider serviceProvider, IOdsCodeLookup odsCodeLookup, ILogger<GpSystemFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _odsCodeLookup = odsCodeLookup ?? throw new ArgumentNullException(nameof(odsCodeLookup));
            _logger = logger;
        }

        public IGpSystem CreateGpSystem(Supplier supplier)
        {
            var gpSystems = _serviceProvider.GetServices<IGpSystem>();

            try
            {
                return gpSystems.Single(b => b.Supplier == supplier);
            }
            catch (InvalidOperationException exception)
            {
                throw new UnknownSupplierException(supplier, exception);
            }
        }

        public async Task<Option<IGpSystem>> LookupGpSystem(string odsCode)
        {
            var supplier = await _odsCodeLookup.LookupSupplier(odsCode);

            try
            {
                _logger.LogInformation($"Fetch GP System: '{supplier}'.");
                return supplier.Select(CreateGpSystem);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Failed to create GP System for supplier: {supplier}.");
                return Option.None<IGpSystem>();
            } 
        }
    }
}