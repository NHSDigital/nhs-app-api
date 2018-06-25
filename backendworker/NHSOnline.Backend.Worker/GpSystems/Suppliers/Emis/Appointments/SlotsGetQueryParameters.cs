using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Appointments
{
    public class SlotsGetQueryParameters
    {
        public SlotsGetQueryParameters(DateTimeOffset fromDate, DateTimeOffset toDate, string userPatientLinkToken)
        {
            FromDateTime = fromDate;
            ToDateTime = toDate;
            UserPatientLinkToken = userPatientLinkToken;
        }

        public string UserPatientLinkToken { get; }
        public DateTimeOffset FromDateTime { get; }
        public DateTimeOffset ToDateTime { get; }
    }
}