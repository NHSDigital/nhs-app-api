using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UserInfo.Repository
{
    [BsonIgnoreExtraElements]
    public class UserAndInfo : RepositoryRecord
    {
        [BsonElement]
        [JsonProperty(PropertyName = "id")]
        public string NhsLoginId { get; set; }

        [BsonElement]
        public Info Info { get; set; }
    }
}