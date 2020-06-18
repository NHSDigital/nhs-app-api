using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    [BsonIgnoreExtraElements]
    public class UserDevice : RepositoryRecord
    {
        [BsonElement]
        public string NhsLoginId { get; set; }

        [BsonId]
        public string DeviceId { get; set; }

        [BsonElement]
        public string RegistrationId { get; set; }

        [BsonElement]
        public string PnsToken { get; set; }
    }
}