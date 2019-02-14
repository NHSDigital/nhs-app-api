using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class ConsultationHeaderItem
    {
        public ConsultationHeaderItem()
        {
            ObservationsWithTerm = new List<ObservationItemWithTerm>();
            AssociatedTexts = new List<string>();
        }
        
        public string Header { get; set; }
        public List<ObservationItemWithTerm> ObservationsWithTerm { get; set; }
        public List<string> AssociatedTexts { get; set; }
    }
}