using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisProxyUserSession
    {
        public Guid Id { get; set; }

        public string UserPatientLinkToken { get; set; }

        public string OdsCode { get; set; }
        
        public string NhsNumber { get; set; }
    }
}
