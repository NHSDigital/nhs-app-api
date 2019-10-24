using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class SlotsMetadataGetQueryParameters
    {
        public SlotsMetadataGetQueryParameters(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            SessionStartDate = fromDate;
            SessionEndDate = toDate;
        }

        public DateTimeOffset SessionStartDate { get;  }
        public DateTimeOffset SessionEndDate { get;  }
    }
}