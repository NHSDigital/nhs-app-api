using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.Backend.Support.Repository
{
    public abstract class MongoRecord
    {
        [BsonElement("_ts")]
        public DateTime Timestamp { get; set; }
    }
}