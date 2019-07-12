using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class TppDcrEvents: IPatientDataModel
    {
        public TppDcrEvents()
        {
            HasAccess = true;
            HasErrored = false;
            Data = new List<TppDcrEvent>();
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public int RecordCount => Data?.Count ?? 0;
        public string Errors { get; set; }
        public List<TppDcrEvent> Data { get; set; }
    }
}