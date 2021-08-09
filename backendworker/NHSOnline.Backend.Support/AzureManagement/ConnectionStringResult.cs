using Newtonsoft.Json;

namespace NHSOnline.Backend.Support.AzureManagement
{
    public class ConnectionStringResult
    {
        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}