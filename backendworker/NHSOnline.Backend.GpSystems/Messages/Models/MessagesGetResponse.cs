using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class MessagesGetResponse
    {
        public List<MessageSummary> Messages { get; set; }
    }
}