namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class CreatePatientMessage
    {
        public string Subject { get; set;  }
        public string MessageBody { get; set; }
        public string Recipient { get; set; }

    }
}