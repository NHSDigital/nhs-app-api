using System;
using System.Runtime.Serialization;

namespace NHSOnline.Backend.Worker.GpSystems
{
    [Serializable]
    public class UnknownSupplierException : Exception
    {
        public UnknownSupplierException(Supplier supplier, Exception innerException)
            : base($"Could not find GP system for supplier ({supplier}).", innerException)
        {
            
        }
        public UnknownSupplierException()
        {
        }

        public UnknownSupplierException(string message) : base(message)
        {
        }

        public UnknownSupplierException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownSupplierException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}