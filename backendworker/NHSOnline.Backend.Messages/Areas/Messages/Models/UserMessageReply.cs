using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    [BsonIgnoreExtraElements]
    public class UserMessageReply
    {
        [BsonElement]
        public List<UserReplyOption> Options { get; set; } = new List<UserReplyOption>();

        [BsonElement]
        public string Response { get; set; }

        [BsonElement]
        public DateTime? ResponseSentDateTime { get; set; }

        [BsonElement]
        public string Status { get; set; }

        [BsonElement]
        public DateTime? ResponseCompletedDateTime { get; set; }
    }
}