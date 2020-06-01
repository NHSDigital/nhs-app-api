using System;
using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.UsersApi.Repository
{
    public class UserDevice : MongoRecord
    {
        [BsonElement]
        public string NhsLoginId { get; set; }

        [BsonId]
        public string DeviceId { get; set; }

        [BsonElement]
        public string RegistrationId { get; set; }

        [BsonElement]
        public DateTime? RegistrationExpiry { get; set; }

        [BsonElement]
        public string PnsToken { get; set; }
    }
}