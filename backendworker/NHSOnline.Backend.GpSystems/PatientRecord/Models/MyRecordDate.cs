using System;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class MyRecordDate
    {
        public DateTimeOffset? Value { get; set; }  
        public string DatePart { get; set; } 
    }
}