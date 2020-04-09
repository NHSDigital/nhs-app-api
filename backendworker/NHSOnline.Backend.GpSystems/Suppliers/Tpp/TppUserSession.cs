using System;
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

        public string UnitId => OdsCode;

        public ICollection<TppProxyUserSession> ProxyPatients { get; set; }

        public Guid? GetCurrentlyAuthenticatedId()
        {
            if (Suid != null)
            {
                return Id;
            }

            return ProxyPatients.FirstOrDefault(x => x.Suid != null)?.Id;
        }
    }
}
