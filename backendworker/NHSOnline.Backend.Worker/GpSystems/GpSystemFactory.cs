using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public class GpSystemFactory : IGpSystemFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GpSystemFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IGpSystem CreateGpSystem(SupplierEnum supplier)
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
    }
}