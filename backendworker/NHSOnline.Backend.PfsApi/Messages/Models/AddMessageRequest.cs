namespace NHSOnline.Backend.PfsApi.Messages.Models
{
    public class AddMessageRequest
    {
        public string Sender { get; set; }
        public int Version { get; set; }
        public string Body { get; set; }
        public AddMessageSenderContext SenderContext { get; set; }
    }
}
