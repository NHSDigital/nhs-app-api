using System;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public class AppointmentSlotsDateRange
    {
        private const int DayRange = 29;

        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
        
        public AppointmentSlotsDateRange(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            var nowDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            FromDate = nowDateTimeOffset;
            ToDate = nowDateTimeOffset.AddDays(DayRange).SetTimeToMidnight();  
        }
    }
}