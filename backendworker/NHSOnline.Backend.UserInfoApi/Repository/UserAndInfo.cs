using MongoDB.Bson.Serialization.Attributes;
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