namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class TppUserSession: UserSession
    {
        public override SupplierEnum Supplier => SupplierEnum.Tpp;
        
        // From the `suid` in the autheticate response
        public string SessionId { get; set; }
    }
}