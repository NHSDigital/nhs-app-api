using System;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Temporal;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class DateRangeValidator: IDateRangeValidator
    {
        private readonly IDateTimeOffsetProvider _dateTimeOffsetProvider;
        
        public DateRangeValidator(IDateTimeOffsetProvider dateTimeOffsetProvider)
        {
            _dateTimeOffsetProvider = dateTimeOffsetProvider;
        }
        
        public bool IsValid(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            if (fromDate == null || toDate == null)
            {
                return true;
            }

            var nowDate = _dateTimeOffsetProvider.CreateDateTimeOffset();
            if (fromDate < nowDate && toDate < nowDate)
            {
                return false;
            }
              
            return (fromDate <= toDate);
        }
    }
}
