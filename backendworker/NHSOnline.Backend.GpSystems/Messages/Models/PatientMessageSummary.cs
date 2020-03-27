namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class PatientMessageSummary
    {
        public string Id { get; set; }

        public string ConversationId { get; set; }
        public string Subject { get; set; }
        public string MessageText { get; set; }
        public string LastMessageDateTime { get; set; }
        public string Recipient { get; set; }
        public int? ReplyCount { get; set; }
        public string Sender { get; set; }
        public int UnreadCount { get; set; }
        public string AttachmentId { get; set; }
        public bool? HasUnreadReplies { get; set; }
    }
}
