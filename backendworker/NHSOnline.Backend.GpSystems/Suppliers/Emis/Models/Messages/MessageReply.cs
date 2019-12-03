using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages
{
    public class MessageReply
    {
        public string Sender { get; set;}
        public string SentDateTime { get; set;}
        public bool IsUnread { get; set; }
        public string ReplyContent { get; set;}
        public bool? isLegacy { get; set; }
        
    }
}