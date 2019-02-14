using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class ExceptionErrorResponse
    {
        public IEnumerable<ErrorResponseExceptionModel> Exceptions { get; set; }
        public int InternalResponseCode { get; set; }
        public string InternalResponseMethod { get; set; }
    }
}
