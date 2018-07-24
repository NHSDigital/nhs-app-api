using System;

namespace NHSOnline.Backend.Worker
{
    [Serializable]
    public abstract class UserSession
    {
        public string Key { get; set; }

        public string NhsNumber { get; set; }

        public abstract SupplierEnum Supplier { get; }

        public string CsrfToken { get; set; }
    }
}