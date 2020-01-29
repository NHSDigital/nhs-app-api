using System.Net.Http;

namespace NHSOnline.Backend.NominatedPharmacy.Clients
{
    public class NominatedPharmacyHttpClient
    {
        public NominatedPharmacyHttpClient(HttpClient client, INominatedPharmacyConfigurationSettings config)
        {
            client.BaseAddress = config.BaseUrl;
            Client = client;

            if (config.BaseUrl == null)
            {
                config.SettingsUpdated += ConfigOnSettingsUpdated;
            }
        }

        private void ConfigOnSettingsUpdated(object sender, NominatedPharmacyConfigurationUpdatedEventArgs e)
        {
            Client.BaseAddress = e.Config.BaseUrl;

            e.Config.SettingsUpdated -= ConfigOnSettingsUpdated;
        }

        public HttpClient Client { get; }
    }
}
