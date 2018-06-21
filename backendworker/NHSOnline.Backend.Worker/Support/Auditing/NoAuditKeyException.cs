using System;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class NoAuditKeyException : Exception
    {
        public NoAuditKeyException(string message) : base(message)
        {
        }
    }
}