using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    [BsonIgnoreExtraElements]
    public class UserAndInfo : MongoRecord
    {
        [BsonElement]
        public string NhsLoginId { get; set; }

        [BsonElement]
        public Info Info { get; set; }
    }
}