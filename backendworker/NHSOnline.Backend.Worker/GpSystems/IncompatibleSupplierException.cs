using System;

namespace NHSOnline.Backend.Worker.GpSystems
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
