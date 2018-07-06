using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class FilterDetails
    {
        public string ItemFilter { get; set; }
        public DateTimeOffset? ItemFilterFromDate { get; set; }
        public DateTimeOffset? ItemFilterToDate { get; set; }
        public DateTimeOffset? FreeTextFilterFromDate { get; set; }
    }
}