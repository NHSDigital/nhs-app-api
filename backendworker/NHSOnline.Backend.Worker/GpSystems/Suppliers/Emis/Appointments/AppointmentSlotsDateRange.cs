using System;
using NHSOnline.Backend.Worker.Support.Date;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class AppointmentSlotsDateRange
    {
        private const int DayRange = 29;

        public DateTimeOffset FromDate { get; }
        public DateTimeOffset ToDate { get; }
        
        public AppointmentSlotsDateRange(IDateTimeOffsetProvider dateTimeOffsetProvider, DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            if (fromDate == null && toDate == null)
            {
                var nowdateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
                FromDate = nowdateTimeOffset;
                ToDate = nowdateTimeOffset.AddDays(DayRange).SetTimeToMidnight();
            }
            else
            {
                FromDate = fromDate ?? toDate.Value.SubDays(DayRange - 1).SetTimeToMidnight();
                ToDate = toDate ?? fromDate.Value.AddDays(DayRange).SetTimeToMidnight();
                FromDate = FromDate;
                ToDate = ToDate;
            }    
        }
    }
}
