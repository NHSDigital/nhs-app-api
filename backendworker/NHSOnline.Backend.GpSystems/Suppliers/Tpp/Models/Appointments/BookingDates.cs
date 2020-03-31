using System;
using System.Globalization;
using NHSOnline.Backend.Support.Temporal;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.Appointments
{
    public class BookingDates
    {
        private const string TppDateTimeFormat = "yyy-MM-ddTHH:mm:ss+00:00";

        public BookingDates(DateTimeOffset? startDate, DateTimeOffset? endDate, IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            StartDate = dateTimeOffsetProvider.ConvertToLocalTime(startDate.Value).ToString(TppDateTimeFormat, CultureInfo.InvariantCulture);
            EndDate = dateTimeOffsetProvider.ConvertToLocalTime(endDate.Value).ToString(TppDateTimeFormat, CultureInfo.InvariantCulture);
        }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}