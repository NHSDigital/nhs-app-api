using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    public class UserAndInfo : MongoRecord
    {
        [BsonId]
        public string NhsLoginId { get; set; }

        [BsonElement]
        public Info Info { get; set; }
    }
}