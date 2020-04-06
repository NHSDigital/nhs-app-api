using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class MessageSummary
    {
        public int MessageId { get; set; }
        public string Subject { get; set; }
        public string SentDateTime { get; set; }
        public int? ReplyCount { get; set; }
        public bool? HasUnreadReplies { get; set; }
        public string LastReplyDateTime { get; set; }
        public string LastReplyFromDisplayName { get; set; }
        public List<UserMessageRecipient> Recipients { get; set; }
    }
}
