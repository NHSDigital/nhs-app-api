using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.GpSystems.Im1Connection.Cache
{
    internal sealed class Im1CacheRecord: RepositoryRecord
    {
        [BsonElement("_id")]
        public string Key { get; set; }

        [BsonElement("token")]
        public string Token { get; set; }

        [BsonElement("doctype")]
        public string DocumentType { get; set; } = "im1connectiontoken";
    }
}