using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
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
