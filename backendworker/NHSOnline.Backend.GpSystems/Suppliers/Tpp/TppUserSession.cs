using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppUserSession: GpUserSession, ITppUserSession
    {
        public override Supplier Supplier => Supplier.Tpp;
        public override bool HasLinkedAccounts => ProxyPatients != null && ProxyPatients.Any();
        public string Suid { get; set; }
        public string PatientId { get; set; }
        public string OnlineUserId { get; set; }
        public bool HasSelfAccess { get; set; }

        public string UnitId => OdsCode;

        public ICollection<TppProxyUserSession> ProxyPatients { get; set; }

        public string GetCurrentlyAuthenticatedId()
        {
            return Suid != null ? PatientId : ProxyPatients?.FirstOrDefault(x => x.Suid != null)?.PatientId;
        }

        public override T Accept<T>(IGpUserSessionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
