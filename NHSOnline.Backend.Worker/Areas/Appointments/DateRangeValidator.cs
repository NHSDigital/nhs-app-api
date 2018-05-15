using System;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class DateRangeValidator: IDateRangeValidator
    {
        public bool IsValid(DateTimeOffset? fromDate, DateTimeOffset? toDate)
        {
            if (fromDate == null || toDate == null)
            {
                return true;
            }
              
            return (fromDate <= toDate);
        }
    }
}
