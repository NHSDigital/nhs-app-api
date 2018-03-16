using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Suppliers.Emis;

namespace NHSOnline.Backend.Worker.Suppliers
{
    public class SystemProviderFactory : ISystemProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public SystemProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ISystemProvider CreateSystemProvider(SupplierEnum supplier)
        {
            switch (supplier)
            {
                case SupplierEnum.Emis:
                    return _serviceProvider.GetService<EmisSystemProvider>();
                default:
                    throw new UnknownSupplierException();
            }
        }
    }
}