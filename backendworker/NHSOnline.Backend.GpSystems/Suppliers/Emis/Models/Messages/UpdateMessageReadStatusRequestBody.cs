namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.Messages
{
    public class UpdateMessageReadStatusRequestBody
    {
        public int MessageId { get; set; }
        public string MessageReadState { get; set; }
    }
}