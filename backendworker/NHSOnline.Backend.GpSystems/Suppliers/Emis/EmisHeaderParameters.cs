namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHeaderParameters
    {
        public EmisHeaderParameters(EmisUserSession emisUserSession)
        {
            EndUserSessionId = emisUserSession.EndUserSessionId;
            SessionId = emisUserSession.SessionId;
        }

        public string EndUserSessionId { get; set; }
        public string SessionId { get; set; }
    }
}