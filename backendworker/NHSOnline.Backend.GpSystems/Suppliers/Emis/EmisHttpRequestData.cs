namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHttpRequestData
    {
        public EmisHttpRequestData(EmisHeaderParameters headerParameters, string userPatientLinkToken)
        {
            HeaderParameters = headerParameters;
            UserPatientLinkToken = userPatientLinkToken;
        }

        public EmisHttpRequestData()
        {
        }
        
        public EmisHeaderParameters HeaderParameters { get; set; }
        
        public string UserPatientLinkToken { get; set; }

    }
}