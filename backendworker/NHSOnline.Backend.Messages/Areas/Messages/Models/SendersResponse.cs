using System.Collections.Generic;

namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class SendersResponse
    {
        public List<Sender> Senders { get; set; } = new List<Sender>();
    }
}