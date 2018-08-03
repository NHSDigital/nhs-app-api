using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class ErrorResponseExceptionModel
    {
        public string CallStack { get; set; }
        public Guid ExceptionId { get; set; }
        public Guid InnerExceptionId { get; set; }
        public bool IsRootException { get; set; }
        public string Message { get; set; }
    }
}