using System;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public class AppointmentSlotsDateRange
    {
        public DateTimeOffset FromDate { get; private set; }
        public DateTimeOffset ToDate { get; private set; }

        private static int EightWeeksDayRange { get; } = 56;    // Eight weeks in days = 8 * 7 = 56.
        private static int SixteenWeeksDayRange { get; } = 112; // Sixteen weeks in days = 16 * 7 = 112.

        public int DayRange { get; private set; }

        public AppointmentSlotsDateRange(IDateTimeOffsetProvider dateTimeOffsetProvider, bool sixteenWeeksSlotsEnabled)
        {
            var nowDateTimeOffset = dateTimeOffsetProvider.CreateDateTimeOffset();

            DayRange = sixteenWeeksSlotsEnabled ? SixteenWeeksDayRange : EightWeeksDayRange;
            FromDate = nowDateTimeOffset;
            ToDate = dateTimeOffsetProvider.CreateDateTimeOffset(nowDateTimeOffset.DateTime.AddDays(DayRange).AddDays(1)).SetTimeToMidnight();
        }

        public AppointmentSlotsDateRange(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
            DayRange = (toDate.DateTime - fromDate.DateTime).Days - 1;
        }
    }
}