using System;

namespace NHSOnline.Backend.Worker.Session
{
    [Serializable]
    public abstract class UserSession
    {
        public string Key { get; set; }

        public abstract SupplierEnum Supplier { get; }
    }
}