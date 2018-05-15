using System;

namespace NHSOnline.Backend.Worker.Router
{
    public interface IDateRangeValidator
    {
        bool IsValid(DateTimeOffset? fromDate, DateTimeOffset? toDate);
    }
}
