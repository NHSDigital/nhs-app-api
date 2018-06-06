using System;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.AppointmentSlots
{
    public class AppointmentSlotsDateRange
    {
        private const int DayRange = 15;

        public DateTimeOffset FromDate { get; }
        public DateTimeOffset ToDate { get; }
        
        public AppointmentSlotsDateRange(IDateTimeOffsetProvider dateTimeOffsetProvider, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            if (fromDate == null && toDate == null)
            {
                var nowdateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
                FromDate = nowdateTimeOffset.ToUniversalTime();
                ToDate = nowdateTimeOffset.AddDays(DayRange).SetTimeToMidnight().ToUniversalTime();
            }
            else
            {
                FromDate = fromDate ?? toDate.Value.SubDays(DayRange - 1).SetTimeToMidnight();
                ToDate = toDate ?? fromDate.Value.AddDays(DayRange).SetTimeToMidnight();
                FromDate = FromDate.ToUniversalTime();
                ToDate = ToDate.ToUniversalTime();
            }    
        }
    }
}
