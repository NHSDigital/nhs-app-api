using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems
{
    public class GpSystemFactory : IGpSystemFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<GpSystemFactory> _logger;

        public GpSystemFactory(IServiceProvider serviceProvider, ILogger<GpSystemFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IGpSystem CreateGpSystem(Supplier supplier)
        {
            var gpSystems = _serviceProvider.GetServices<IGpSystem>();

            try
            {
                _logger.LogInformation($"Fetch GP System: '{supplier}'.");
                return gpSystems.Single(b => b.Supplier == supplier);
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogWarning($"Failed to create GP System for supplier: '{supplier}'.");
                throw new UnknownSupplierException(supplier, exception);
            }
        }
    }
}