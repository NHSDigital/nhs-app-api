using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisUserSession : GpUserSession
    {
        public override Supplier Supplier => Supplier.Emis;
        public override bool HasLinkedAccounts => ProxyPatients != null && ProxyPatients.Any();
        public string SessionId { get; set; }
        public string EndUserSessionId { get; set; }
        public string UserPatientLinkToken { get; set; }
        public Necessity AppointmentBookingReasonNecessity { get; set; }
        public Necessity PrescriptionSpecialRequestNecessity { get; set; }
        public ICollection<EmisProxyUserSession> ProxyPatients { get; set; }

        public override T Accept<T>(IGpUserSessionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
