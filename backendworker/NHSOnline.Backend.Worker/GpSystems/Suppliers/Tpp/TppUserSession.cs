using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppUserSession: GpUserSession, ITppUserSession
    {
        public override Supplier Supplier => Supplier.Tpp;
        
        public string Suid { get; set; }
        public string PatientId { get; set; }
        public string OnlineUserId { get; set; }

        public string UnitId => OdsCode;
    }
}
