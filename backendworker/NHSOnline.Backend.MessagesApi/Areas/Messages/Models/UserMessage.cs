using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class UserMessage : MongoRecord
    {
        [BsonElement]
        public string NhsLoginId { get; set; }

        [BsonElement]
        public string Sender { get; set; }
        
        [BsonElement]
        public int Version { get; set; }

        [BsonElement]
        public string Body { get; set; }
    }
}