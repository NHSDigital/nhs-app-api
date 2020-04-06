namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class UpdateMessageReadStatusRequest
    {
        public string UserPatientLinkToken { get; set; }
        public int MessageId { get; set; }
        public string MessageReadState { get; set; }
    }
}