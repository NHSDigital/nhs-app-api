using System;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class AddMessageSenderContext
    {
        public string SupplierId { get; set; }

        public string CommunicationId { get; set; }

        public string TransmissionId { get; set; }

        public DateTime? CommunicationCreatedDateTime { get; set; }

        public string RequestReference { get; set; }

        public string CampaignId { get; set; }

        public string OdsCode { get; set; }

        public string NhsNumber { get; set; }

        public string NhsLoginId { get; set; }
    }
}