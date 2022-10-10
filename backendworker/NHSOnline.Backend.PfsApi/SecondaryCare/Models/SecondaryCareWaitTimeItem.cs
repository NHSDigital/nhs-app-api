using System;
using System.Globalization;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class SecondaryCareWaitTimeItem
    {
        public DateTimeOffset ReferredDate { get; set; }

        public decimal PlannedWaitTime { get; set; }

        public string ProviderName { get; set; }

        public string Speciality { get; set; }

        public string EstimatedWaitToTreatmentDisplayDate =>
            ConvertReferredDateToEstimatedWaitToTreatmentDisplayDate();

        private string ConvertReferredDateToEstimatedWaitToTreatmentDisplayDate()
        {
            return ReferredDate.AddDays(Decimal.ToDouble(PlannedWaitTime))
                .ToString("MMMM yyyy", CultureInfo.InvariantCulture);
        }
    }
}