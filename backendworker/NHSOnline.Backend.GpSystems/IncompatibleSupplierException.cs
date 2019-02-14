using System;

namespace NHSOnline.Backend.GpSystems
{
    public class IncompatibleSupplierException : Exception
    {
        public IncompatibleSupplierException(string message) : base(message)
        {
        }

        public IncompatibleSupplierException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IncompatibleSupplierException()
        {
        }
    }
}
