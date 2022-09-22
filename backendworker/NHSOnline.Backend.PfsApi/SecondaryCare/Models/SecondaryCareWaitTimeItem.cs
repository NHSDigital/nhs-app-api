using System;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class SecondaryCareWaitTimeItem
    {
        public DateTimeOffset ReferredDate { get; set; }

        public decimal PlannedWaitTime { get; set; }

        public string ProviderName { get; set; }

        public string Speciality { get; set; }
    }
}