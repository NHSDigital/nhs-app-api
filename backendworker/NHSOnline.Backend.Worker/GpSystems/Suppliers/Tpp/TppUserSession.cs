namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppUserSession: UserSession
    {
        public override SupplierEnum Supplier => SupplierEnum.Tpp;
        
        public string Suid { get; set; }
        public string PatientId { get; set; }
        public string UnitId { get; set; }
        public string OnlineUserId { get; set; }
    }
}