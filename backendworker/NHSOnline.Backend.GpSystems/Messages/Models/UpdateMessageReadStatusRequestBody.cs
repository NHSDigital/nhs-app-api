namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class UpdateMessageReadStatusRequestBody
    {
        public string MessageId { get; set; }
        public string MessageReadState { get; set; }
    }
}
