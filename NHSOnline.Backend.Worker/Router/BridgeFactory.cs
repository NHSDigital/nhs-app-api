using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Worker.Bridges.Emis;

namespace NHSOnline.Backend.Worker.Router
{
    public class BridgeFactory : IBridgeFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BridgeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IBridge CreateBridge(SupplierEnum supplier)
        {
            switch (supplier)
            {
                case SupplierEnum.Emis:
                    return _serviceProvider.GetService<EmisBridge>();
                default:
                    throw new UnknownSupplierException();
            }
        }
    }
}