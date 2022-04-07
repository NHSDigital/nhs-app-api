using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UserInfo.Repository
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