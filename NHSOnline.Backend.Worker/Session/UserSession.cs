using System;

namespace NHSOnline.Backend.Worker.Session
{
    [Serializable]
    public abstract class UserSession
    {
        public abstract SupplierEnum Supplier { get; }
    }
}