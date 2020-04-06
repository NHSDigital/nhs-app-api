using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class PatientMessageDetails
    {
        public int? MessageId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string SentDateTime { get; set; }
        public string Recipient { get; set; }
        public bool? OutboundMessage { get; set; }
        public List<MessageReply> MessageReplies { get; set; }
    }
}