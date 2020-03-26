namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppRequestParameters
    {
        public TppRequestParameters(TppUserSession tppUserSession)
        {
            PatientId = tppUserSession.PatientId;
            OdsCode = tppUserSession.OdsCode;
            OnlineUserId = tppUserSession.OnlineUserId;
            Suid = tppUserSession.Suid;
        }

        public TppRequestParameters()
        {
        }
        public string PatientId { get; set; }
        public string OnlineUserId { get; set; }
        public string OdsCode { get; set; }
        public string Suid { get; set; }
    }
}