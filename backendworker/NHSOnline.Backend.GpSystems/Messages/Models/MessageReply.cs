namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class MessageReply
    {
        public string Sender { get; set;}
        public string SentDateTime { get; set;}
        public bool IsUnread { get; set; }
        public string ReplyContent { get; set;}
        public bool? OutboundMessage { get; set; }
        public string AttachmentId { get; set; }
    }
}