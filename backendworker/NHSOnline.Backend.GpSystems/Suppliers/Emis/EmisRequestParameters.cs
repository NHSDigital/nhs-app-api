namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisRequestParameters
    {
        public EmisRequestParameters(EmisUserSession emisUserSession)
        {
            EndUserSessionId = emisUserSession.EndUserSessionId;
            SessionId = emisUserSession.SessionId;
            UserPatientLinkToken = emisUserSession.UserPatientLinkToken;
        }
        
        public EmisRequestParameters(EmisUserSession emisUserSession, string userPatientLinkToken)
        {
            EndUserSessionId = emisUserSession.EndUserSessionId;
            SessionId = emisUserSession.SessionId;
            UserPatientLinkToken = userPatientLinkToken;
        }

        public EmisRequestParameters()
        {
        }

        public string EndUserSessionId { get; set; }
        public string SessionId { get; set; }
        
        public string UserPatientLinkToken { get; set; }
        
    }
}