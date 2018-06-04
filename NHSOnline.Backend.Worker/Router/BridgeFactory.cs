using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

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
            var bridges = _serviceProvider.GetServices<IBridge>();

            try
            {
                return bridges.Single(b => b.Supplier == supplier);
            }
            catch (InvalidOperationException)
            {
                throw new UnknownSupplierException();
            }
        }
    }
}