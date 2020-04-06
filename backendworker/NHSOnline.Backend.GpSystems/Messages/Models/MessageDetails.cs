using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class MessageDetails
    {
        public string ClientApplicationName { get; set; }
        public int? MessageId { get; set; }
        public string SentDateTime { get; set;}
        public string Subject { get; set;}
        public string Content { get; set;}
        public List<UserMessageRecipient> Recipients { get; set; }
        public List<MessageReply> MessageReplies { get; set; }
    }
}