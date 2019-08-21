using System;

namespace NHSOnline.Backend.Support.AspNet.Filters
{
    public class NhsTimeoutException : TimeoutException
    {
        public SourceApi SourceApi { get; set; }

        public NhsTimeoutException()
        {
        }

        public NhsTimeoutException(string message) : base(message)
        {
        }

        public NhsTimeoutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}