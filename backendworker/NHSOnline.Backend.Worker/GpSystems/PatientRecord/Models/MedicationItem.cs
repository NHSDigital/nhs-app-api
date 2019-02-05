using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models
{
    public class MedicationItem
    {
        public MedicationItem()
        {
            LineItems = new List<MedicationLineItem>();
        }
        public DateTimeOffset? Date { get; set; }
        public List<MedicationLineItem> LineItems { get; set; }
    }
}