namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppUserSession: UserSession
    {
        public override Supplier Supplier => Supplier.Tpp;
        
        public string Suid { get; set; }
        public string PatientId { get; set; }
        public string OnlineUserId { get; set; }
        public string AccessToken { get; set; }
    }
}
