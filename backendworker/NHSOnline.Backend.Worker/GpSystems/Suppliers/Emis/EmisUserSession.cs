namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisUserSession: UserSession
    {
        public override SupplierEnum Supplier => SupplierEnum.Emis;
        public string SessionId { get; set; }
        public string EndUserSessionId { get; set; }
        public string UserPatientLinkToken { get; set; }
    }
}