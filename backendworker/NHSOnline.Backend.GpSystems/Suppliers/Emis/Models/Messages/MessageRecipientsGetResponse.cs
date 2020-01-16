using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages
{
    public class MessageRecipientsGetResponse
    {
        public MessageRecipientsGetResponse()
        {
            MessageRecipients = new List<MessageRecipient>();
        }
        
        public List<MessageRecipient> MessageRecipients { get; set; }
    }
}