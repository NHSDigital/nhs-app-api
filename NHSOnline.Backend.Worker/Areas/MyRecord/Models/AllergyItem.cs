using System;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class AllergyItem
    {
        public string AllergyName { get; set; }
        
        public DateTimeOffset AvailabilityDate { get; set; }
    }
}