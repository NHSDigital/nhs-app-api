using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Mocking.Emis.Models
{
    public class ExceptionResponse
    {
        public long InternalResponseCode { get; }
        public string InternalResponseMessage => "Exception occurred during API processing.";
        public IEnumerable<Exception> Exceptions { get; }


        public ExceptionResponse(long internalResponseCode, string exceptionMessage)
        {
            InternalResponseCode = internalResponseCode;
            Exceptions = new[] { new Exception(exceptionMessage) };
        }

        public class Exception
        {
            public string CallStack { get; }
            public string ExceptionId { get; }
            public string InnerExceptionId { get; }
            public bool IsRootException { get; }
            public string Message { get; }

            public Exception(string message)
            {
                CallStack = "Not available";
                ExceptionId = "00000000-0000-0000-0000-000000000000";
                InnerExceptionId = "00000000-0000-0000-0000-000000000000";
                IsRootException = true;
                Message = message;
            }
        }
    }


}
