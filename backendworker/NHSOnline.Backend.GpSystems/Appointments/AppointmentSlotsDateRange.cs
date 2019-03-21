using System;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public class AppointmentSlotsDateRange
    {
        public DateTimeOffset FromDate { get; private set; }
        public DateTimeOffset ToDate { get; private set; }

        public int DayRange { get; } = 29;

        public AppointmentSlotsDateRange(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            var nowDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();
            FromDate = nowDateTimeOffset;
            ToDate = dateTimeOffsetProvider.CreateDateTimeOffset(nowDateTimeOffset.DateTime.AddDays(DayRange)).SetTimeToMidnight();
        }

        public AppointmentSlotsDateRange(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
            DayRange = (toDate.DateTime - fromDate.DateTime).Days;
        }
    }
}