using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class PatientPracticeMessageRecipients
    {
        public List<MessageRecipient> MessageRecipients { get; set; }
        public bool HasErrored { get; set; }
    }
}