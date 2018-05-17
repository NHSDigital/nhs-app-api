namespace NHSOnline.Backend.Worker.Bridges.Emis
{
    public class EmisUserSession: UserSession
    {
        public override SupplierEnum Supplier => SupplierEnum.Emis;
        public string SessionId { get; set; }
        public string EndUserSessionId { get; set; }
        public string UserPatientLinkToken { get; set; }
    }
}