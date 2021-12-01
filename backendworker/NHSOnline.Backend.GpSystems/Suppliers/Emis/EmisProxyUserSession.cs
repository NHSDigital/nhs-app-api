using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisProxyUserSession
    {
        /// <summary>
        /// To be removed - jira 13005
        /// </summary>
        public Guid Id { get; set; }

        public string UserPatientLinkToken { get; set; }

        public string OdsCode { get; set; }

        public string NhsNumber { get; set; }

        public string PatientActivityContextGuid { get; set; }
    }
}
