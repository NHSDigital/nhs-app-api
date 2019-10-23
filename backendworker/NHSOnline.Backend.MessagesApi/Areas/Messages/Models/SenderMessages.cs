using System.Collections.Generic;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class SenderMessages
    {
        public string Sender { get; set; }
        
        public int UnreadCount { get; set; }
        
        public List<Message> Messages { get; set; }
    }
}