using System;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class ErrorResponseException
    {
        public string CallStack { get; set; }
        public Guid ExceptionId { get; set; }
        public Guid InnerExceptionId { get; set; }
        public bool IsRootException { get; set; }
        public string Message { get; set; }
    }
}