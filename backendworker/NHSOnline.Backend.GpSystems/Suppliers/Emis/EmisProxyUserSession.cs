namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisProxyUserSession
    {
        public string UserPatientLinkToken { get; set; }

        public string OdsCode { get; set; }

        public string NhsNumber { get; set; }

        public string PatientActivityContextGuid { get; set; }
    }
}
