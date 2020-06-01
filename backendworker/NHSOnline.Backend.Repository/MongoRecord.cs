using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.Backend.Repository
{
    public abstract class MongoRecord
    {
        [BsonElement("_ts")]
        public DateTime Timestamp { get; set; }
    }
}