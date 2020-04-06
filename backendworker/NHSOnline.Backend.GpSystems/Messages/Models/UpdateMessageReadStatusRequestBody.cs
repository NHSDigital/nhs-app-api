namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class UpdateMessageReadStatusRequestBody
    {
        public int MessageId { get; set; }
        public string MessageReadState { get; set; }
    }
}