using System;

namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{
    public class MessageLinkClickedRequest
    {
        public Uri Link { get; set; }

        public override string ToString() => $"[Link: '{Link?.AbsoluteUri}']";
    }
}
