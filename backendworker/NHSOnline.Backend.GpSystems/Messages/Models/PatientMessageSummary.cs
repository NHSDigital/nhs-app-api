using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class PatientMessageSummary
    {
        public string MessageId { get; set; }

        public string ConversationId { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string LastMessageDateTime { get; set; }
        public string SentDateTime { get; set; }
        public string Recipient { get; set; }
        public int? ReplyCount { get; set; }
        public string Sender { get; set; }
        public int UnreadCount { get; set; }
        public string AttachmentId { get; set; }
        public bool? HasUnreadReplies { get; set; }
        public List<MessageReply> Replies { get; set; }
        public bool? OutboundMessage { get; set; }
    }
}
