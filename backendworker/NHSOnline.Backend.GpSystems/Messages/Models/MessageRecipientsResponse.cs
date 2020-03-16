using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class MessageRecipientsResponse
    {
        public List<MessageRecipient> MessageRecipients { get; set; }
        public bool HasErrored { get; set; }
    }
}