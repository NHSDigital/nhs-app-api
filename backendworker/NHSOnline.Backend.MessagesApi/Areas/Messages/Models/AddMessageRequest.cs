namespace NHSOnline.Backend.MessagesApi.Areas.Messages.Models
{ 
    public class AddMessageRequest
    {
        public string Sender { get; set; }
        public int Version { get; set; }
        public string Body { get; set; }
        public string CommunicationId { get; set; }
    }
}