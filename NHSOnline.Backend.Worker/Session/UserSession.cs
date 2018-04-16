using System;

namespace NHSOnline.Backend.Worker.Session
{
    [Serializable]
    public class UserSession
    {
        public SupplierEnum Supplier { get; set; }
        public string SupplierSessionId { get; set; }
    }
}