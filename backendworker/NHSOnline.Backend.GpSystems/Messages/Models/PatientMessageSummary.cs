namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class PatientMessageSummary
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string LastMessageDateTime { get; set; }
        public string Recipient { get; set; }
        public int? ReplyCount { get; set; }
        public bool? HasUnreadReplies { get; set; }
    }
}