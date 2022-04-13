using System.Collections.Generic;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class MessagesResponse
    {
        public List<SenderMessages> SenderMessages { get; set; } = new List<SenderMessages>();
    }
}