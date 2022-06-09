using Newtonsoft.Json;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Messages.Repository
{
    public class DbSender : RepositoryRecord
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}