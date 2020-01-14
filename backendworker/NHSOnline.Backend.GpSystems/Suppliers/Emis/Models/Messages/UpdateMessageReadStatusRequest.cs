namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages
{
    public class UpdateMessageReadStatusRequest
    {
        public string UserPatientLinkToken { get; set; }
        public int MessageId { get; set; }
        public string MessageReadState { get; set; }
    }
}