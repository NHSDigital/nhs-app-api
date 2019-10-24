using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class SlotsGetQueryParameters
    {
        public SlotsGetQueryParameters(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            FromDateTime = fromDate;
            ToDateTime = toDate;
        }

        public DateTimeOffset FromDateTime { get; }
        public DateTimeOffset ToDateTime { get; }
    }
}