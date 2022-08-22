using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    [BsonIgnoreExtraElements]
    public class UserReplyOption
    {
        [BsonElement]
        public string Code { get; set; }

        [BsonElement]
        public string Display { get; set; }
    }
}