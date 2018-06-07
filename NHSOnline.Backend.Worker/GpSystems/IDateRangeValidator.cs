using System;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public interface IDateRangeValidator
    {
        bool IsValid(DateTimeOffset? fromDate, DateTimeOffset? toDate);
    }
}
