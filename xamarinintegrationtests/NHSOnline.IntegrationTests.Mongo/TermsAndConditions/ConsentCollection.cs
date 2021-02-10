using MongoDB.Driver;
using NHSOnline.HttpMocks.Domain;

namespace NHSOnline.IntegrationTests.Mongo.TermsAndConditions
{

    public static class ConsentCollection
    {
        private static IMongoCollection<ConsentRecord> SetUpAndRetrieveMongoCollection()
        {
            var mongoUrl = new MongoUrl("mongodb://mongodb.bitraft.io:27017");
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            var mongo = new MongoClient(mongoClientSettings);
            var collection = mongo.GetDatabase("development").GetCollection<ConsentRecord>("consent");
            return collection;
        }

        public static void Add(ConsentRecord consentRecord)
        {
            SetUpAndRetrieveMongoCollection().InsertOne(consentRecord);
        }

        public static ConsentRecord ToConsent(this Patient patient)
        {
            return new()
            {
                NhsLoginId = patient.Id,
                ConsentGiven = true,
                AnalyticsCookieAccepted = true,
                DateOfConsent = "2021-01-11T00:00:00+00:00",
                DateOfAnalyticsCookieToggle = "2021-01-11T00:00:00+00:00"
            };
        }
    }
}