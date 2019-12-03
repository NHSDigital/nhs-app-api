using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages
{
    public class MessagesGetResponse
    {
        public List<MessageSummary> Messages { get; set; }
    }
}