using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class UserMessage : MongoRecord
    {
        [BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string NhsLoginId { get; set; }

        [BsonElement]
        public string Sender { get; set; }
        
        [BsonElement]
        public int Version { get; set; }

        [BsonElement]
        public string Body { get; set; }

        [BsonElement]
        public DateTime SentTime { get; set; }
    }
}