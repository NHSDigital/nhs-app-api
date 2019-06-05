using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.NominatedPharmacy.Clients;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public static class NominatedPharmacyStartup
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpClient<NominatedPharmacyHttpClient>();
            serviceCollection.AddTransient<INominatedPharmacyService, NominatedPharmacyService>();
            serviceCollection.AddSingleton<INominatedPharmacyClient, NominatedPharmacyClient>();
            serviceCollection.AddSingleton<INominatedPharmacyPDSClient, NominatedPharmacyPDSClient>();
            serviceCollection.AddSingleton<INominatedPharmacySubmitClient, NominatedPharmacySubmitClient>();
            serviceCollection.AddTransient<INominatedPharmacyGatewayUpdateService, NominatedPharmacyGatewayUpdateService>();
            serviceCollection.AddTransient<INominatedPharmacyEnvelopeService, NominatedPharmacyEnvelopeService>();
        }
    }
}
