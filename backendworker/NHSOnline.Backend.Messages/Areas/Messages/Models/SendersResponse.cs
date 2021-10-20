using System.Collections.Generic;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class SendersResponse
    {
        public List<Sender> Senders { get; set; } = new List<Sender>();
    }
}