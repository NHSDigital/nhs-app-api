using System;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class MessageLink
    {
        public string MessageId { get; set; }

        public Uri Link { get; set; }

        public override string ToString() => $"MessageId: {MessageId}, Link: {Link?.AbsoluteUri}";
    }
}
