namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class MessageLinkClickedRequest
    {
        public string Link { get; set; }

        public override string ToString() => $"[Link: '{Link}']";
    }
}
