using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UserInfoApi.Repository
{
    [BsonIgnoreExtraElements]
    public class UserAndInfo : RepositoryRecord
    {
        [BsonElement]
        public string NhsLoginId { get; set; }

        [BsonElement]
        public Info Info { get; set; }
    }
}