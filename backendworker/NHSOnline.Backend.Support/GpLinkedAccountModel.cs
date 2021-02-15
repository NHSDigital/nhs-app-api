namespace NHSOnline.Backend.Support
{
    public class GpLinkedAccountModel
    {
        public GpLinkedAccountModel(
            GpUserSession gpUserSession) : this(gpUserSession, string.Empty)
        {
        }

        public GpLinkedAccountModel(
            GpUserSession gpUserSession,
            string requestedPatientId)
        {
            GpUserSession = gpUserSession;
            RequestingPatientGpIdentifier = requestedPatientId ?? string.Empty;
        }

        public GpUserSession GpUserSession { get; }
        public string RequestingPatientGpIdentifier { get; }
    }
}
