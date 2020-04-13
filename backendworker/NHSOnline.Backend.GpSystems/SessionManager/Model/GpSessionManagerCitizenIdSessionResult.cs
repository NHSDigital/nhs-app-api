namespace NHSOnline.Backend.GpSystems.SessionManager.Model
{
    public class GpSessionManagerCitizenIdSessionResult
    {
        public string OdsCode { get; set; }
        public string Im1ConnectionToken { get; set; }
        public string NhsNumber { get; set; }
        public GpSessionManagerCitizenIdUserSession Session { get; set; }
    }
}