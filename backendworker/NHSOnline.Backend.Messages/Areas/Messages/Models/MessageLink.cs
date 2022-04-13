namespace NHSOnline.Backend.Messages.Areas.Messages.Models
{
    public class MessageLink
    {
        public string MessageId { get; set; }

        public string Link { get; set; }

        public override string ToString() => $"MessageId: {MessageId}, Link: {Link}";
    }
}
