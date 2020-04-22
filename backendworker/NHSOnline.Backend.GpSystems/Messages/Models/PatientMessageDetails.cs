using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class PatientMessageDetails
    {
        public string MessageId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string SentDateTime { get; set; }
        public string Recipient { get; set; }
        public bool? OutboundMessage { get; set; }
        public List<MessageReply> Replies { get; set; }
    }
}