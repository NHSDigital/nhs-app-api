using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.Backend.UserInfo.Repository
{
    [BsonIgnoreExtraElements]
    public class Info
    {
        [BsonElement]
        public string OdsCode { get; set; }

        [BsonElement]
        public string NhsNumber { get; set; }
    }
}