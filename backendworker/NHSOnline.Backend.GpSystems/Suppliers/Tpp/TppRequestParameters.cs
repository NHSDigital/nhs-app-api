namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppRequestParameters
    {
        private TppRequestParameters(
            string odsCode,
            string onlineUserId,
            string patientId,
            string suid)
        {
            OdsCode = odsCode;
            OnlineUserId = onlineUserId;
            PatientId = patientId;
            Suid = suid;
        }

        public TppRequestParameters(TppUserSession tppUserSession) :
            this(tppUserSession.OdsCode, tppUserSession.OnlineUserId, tppUserSession.PatientId, tppUserSession.Suid) {}

        public TppRequestParameters(TppUserSession tppUserSession, TppProxyUserSession proxySession) :
            this(tppUserSession.OdsCode, tppUserSession.OnlineUserId, proxySession.PatientId, proxySession.Suid) {}

        public TppRequestParameters()
        {
        }

        public string PatientId { get; set; }
        public string OnlineUserId { get; set; }
        public string OdsCode { get; set; }
        public string Suid { get; set; }
    }
}