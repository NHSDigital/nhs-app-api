using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.PfsApi.TermsAndConditions
{
    [BsonIgnoreExtraElements]
    public class TermsAndConditionsRecord: MongoRecord
    {
        [BsonElement]
        public string NhsLoginId { get; set; }

        [BsonElement]
        public bool ConsentGiven { get; set; }

        [BsonElement]
        public bool AnalyticsCookieAccepted { get; set; }

        [BsonElement]
        public string DateOfConsent { get; set; }

        [BsonElement]
        public string DateOfAnalyticsCookieToggle { get; set; }
    }
}