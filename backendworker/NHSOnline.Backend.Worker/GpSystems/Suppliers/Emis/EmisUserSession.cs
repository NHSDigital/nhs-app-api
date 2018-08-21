namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisUserSession: UserSession
    {
        public override Supplier Supplier => Supplier.Emis;
        public string SessionId { get; set; }
        public string EndUserSessionId { get; set; }
        public string UserPatientLinkToken { get; set; }
        public string OdsCode { get; set; }
    }
}