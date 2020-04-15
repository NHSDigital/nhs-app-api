using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Session
{
    internal sealed class TppScopedLogMessagingService : ITppLogMessagingService
    {
        private readonly IFireAndForgetService _fireAndForgetService;

        public TppScopedLogMessagingService(
            IFireAndForgetService fireAndForgetService)
        {
            _fireAndForgetService = fireAndForgetService;
        }

        public void FetchAndLogAccessInformation(TppUserSession userSession)
        {
            _fireAndForgetService.Run(
                (serviceProvider) => LogAccessInformation(serviceProvider, userSession),
                "Failed request to get list of service accesses");
        }

        private async Task LogAccessInformation(IServiceProvider serviceProvider, TppUserSession userSession)
        {
            var tppAccessInformationService = serviceProvider.GetRequiredService<TppLogMessagingService>();
            await tppAccessInformationService.FetchAndLogAccessInformation(userSession);
        }
    }
}
