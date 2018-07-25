using System;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public class UnknownSupplierException : Exception
    {
        public UnknownSupplierException(SupplierEnum supplier, Exception innerException)
            : base($"Could not find GP system for supplier ({supplier}).", innerException)
        {
            
        }
    }
}