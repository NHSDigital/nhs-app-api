using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class SlotsMetadataGetQueryParameters
    {
        public SlotsMetadataGetQueryParameters(DateTimeOffset fromDate, DateTimeOffset toDate, string userPatientLinkToken)
        {
            SessionStartDate = fromDate;
            SessionEndDate = toDate;
            UserPatientLinkToken = userPatientLinkToken;
        }

        public string UserPatientLinkToken { get;}
        public DateTimeOffset SessionStartDate { get;  }
        public DateTimeOffset SessionEndDate { get;  }
    }
}