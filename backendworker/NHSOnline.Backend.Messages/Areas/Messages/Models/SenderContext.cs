using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class SenderContext
    {
        [BsonElement]
        public string SupplierId { get; set; }

        [BsonElement]
        public string CommunicationId { get; set; }

        [BsonElement]
        public string TransmissionId { get; set; }

        [BsonElement]
        public DateTime? CommunicationCreatedDateTime { get; set; }

        [BsonElement]
        public string RequestReference { get; set; }

        [BsonElement]
        public string CampaignId { get; set; }

        [BsonElement]
        public string OdsCode { get; set; }

        [BsonElement]
        public string NhsNumber { get; set; }

        [BsonElement]
        public string NhsLoginId { get; set; }
    }
}