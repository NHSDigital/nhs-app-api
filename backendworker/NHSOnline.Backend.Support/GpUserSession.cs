using System;

namespace NHSOnline.Backend.Support
{
    [Serializable]
    public abstract class GpUserSession
    {
        public string Name { get; set; }

        public string NhsNumber { get; set; }

        public string OdsCode { get; set; }

        public abstract Supplier Supplier { get; }

        public abstract bool HasLinkedAccounts { get; }

        public bool Im1MessagingEnabled { get; set; }

        public Guid Id { get; set; }

        public abstract T Accept<T>(IGpUserSessionVisitor<T> visitor);
    }
}