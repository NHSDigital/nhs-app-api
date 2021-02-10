using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.IntegrationTests.Mongo.TermsAndConditions
{
    [BsonIgnoreExtraElements]
    public record ConsentRecord : RepositoryRecord
    {
        [BsonElement] public string NhsLoginId { get; init; } = string.Empty;

        [BsonElement] public bool ConsentGiven { get; init; }

        [BsonElement] public bool AnalyticsCookieAccepted { get; init; }

        [BsonElement] public string DateOfConsent { get; init; } = string.Empty;

        [BsonElement] public string DateOfAnalyticsCookieToggle { get; init; } = string.Empty;
    }
}