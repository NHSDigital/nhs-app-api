using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.IntegrationTests.Mongo.TermsAndConditions
{
    public record RepositoryRecord
    {
        [BsonElement("_ts")]
        public DateTime Timestamp { get; set; }
    }
}