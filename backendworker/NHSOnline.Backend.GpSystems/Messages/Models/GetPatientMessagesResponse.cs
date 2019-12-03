using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Messages.Models
{
    public class GetPatientMessagesResponse
    {
        public GetPatientMessagesResponse()
        {
            MessageSummaries = new List<PatientMessageSummary>();
        }
        
        public List<PatientMessageSummary> MessageSummaries { get; set; }
    }
}