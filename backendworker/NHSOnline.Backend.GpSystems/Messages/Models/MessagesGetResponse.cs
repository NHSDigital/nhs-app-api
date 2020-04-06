using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class MessagesGetResponse
    {
        public List<MessageSummary> Messages { get; set; }
    }
}