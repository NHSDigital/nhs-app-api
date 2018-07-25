using System;

namespace NHSOnline.Backend.Worker.GpSystems
{
    public class UnknownSupplierException : Exception
    {
        public UnknownSupplierException(SupplierEnum supplier, Exception innerException): 
            base($"The specified supplier ({supplier}) cannot be found", innerException)
        {
            
        }
    }
}