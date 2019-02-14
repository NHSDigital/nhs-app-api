using System;

namespace NHSOnline.Backend.Support.Http
{
    public class UnauthorisedGpSystemHttpRequestException : Exception
    {
        public UnauthorisedGpSystemHttpRequestException(string message) : base(message)
        {
        }

        public UnauthorisedGpSystemHttpRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnauthorisedGpSystemHttpRequestException()
        {
        }
    }
}
